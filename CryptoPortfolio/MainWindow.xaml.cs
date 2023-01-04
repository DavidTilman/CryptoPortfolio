using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Text.Json;

namespace CryptoPortfolio
{
    public partial class MainWindow : Window
    {
        public DateTime time_of_last_update;
        public MainWindow()
        {
            InitializeComponent();
            Repeat(UpdatePortfolio, new TimeSpan(0,0,0,0,500));

        }
        private void Repeat(EventHandler event_handler, TimeSpan delay)
        {
            DispatcherTimer dispatcherTimer = new()
            {
                Interval = delay
            };
            dispatcherTimer.Tick += new EventHandler(event_handler);
            dispatcherTimer.Start();
            time_of_last_update = DateTime.Now;
        }
        private void UpdatePortfolio(object? _, EventArgs? __)
        {
            if (DateTime.Now - time_of_last_update > AppData.UpdateRate) 
            {
                time_of_last_update = DateTime.Now;
                ApiServicer.CallToJsonFile(
                    ApiQueryBuilder.GetPriceQuery(
                        PortfolioManager.GetCoinsContained()["Symbols"],
                        PortfolioManager.GetCoinsContained()["Currencies"]
                    ),
                    AppData.current_api_call_path
                );
                CrytpoPriceApiResponse response = new(
                    JsonServicer.ReadFile(AppData.current_api_call_path),
                    PortfolioManager.GetCoinsContained()["Symbols"],
                    PortfolioManager.GetCoinsContained()["Currencies"]
                );
                if (response.HasError)
                {
                    MessageBox.Show(response.ErrorMessage);
                    return;
                }
                PortfolioValueDisplay.UpdateCryptoValueDisplay();
                PortfolioValueDisplay.UpdateCurrencyValueDisplay();
                PortfolioViewer.PortfolioViewerDataGrid.DataContext = PortfolioManager.GetPortfolioData();
                File.WriteAllText(
                    AppData.portfolio_last_data_path,
                    JsonSerializer.Serialize(
                        new Dictionary<string, double>() {
                        { "LastCrypto", PortfolioManager.GetValueCurrent(PortfolioValueDisplay.DisplaySymbol) },
                        { "LastCurrency", PortfolioManager.GetValueCurrent(PortfolioValueDisplay.DisplayCurrency) }
                        }
                    )
                );
            }
            UpdateTimeProgressBar.Value = ((DateTime.Now - time_of_last_update) / AppData.UpdateRate) * 100;
        }
        private void AddToPortfolioButton_Click(object sender, RoutedEventArgs e)
        {
            string symbol = SymbolTextBox.Text;
            string quantity_s = QuantityTextBox.Text;
            string currency = CurrencyTextBox.Text;
            string price_s = PriceTextBox.Text;
            if (!double.TryParse(quantity_s, out double quantity))
            {
                MessageBox.Show("Quantity must be a number.");
                return;
            }
            if (!double.TryParse(price_s, out double price))
            {
                MessageBox.Show("Price must be a number.");
                return;
            }
            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be greater than 0.");
                return;
            }
            if (price < 0)
            {
                MessageBox.Show("Price must be greater than or equal to 0.");
                return;
            }

            price = price == -0 ? 0 : price; // werid bug

            PortfolioManager.Add(
                new PortfolioCoin(
                    symbol,
                    quantity,
                    currency,
                    price
            ));

            PortfolioValueDisplay.UpdateCryptoValueDisplay();
            PortfolioValueDisplay.UpdateCurrencyValueDisplay();
        }

        private void DisplayCryptoComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayCryptoComboBox.ItemsSource = AppData.display_cryptos;
            PortfolioValueDisplay.UpdateCryptoValueDisplay();

        }

        private void DisplayCurrencyComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayCurrencyComboBox.ItemsSource = AppData.display_currencies;
        }

        private void DisplayCryptoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PortfolioValueDisplay.DisplaySymbol = DisplayCryptoComboBox.SelectedValue.ToString()!;
            PortfolioViewer.PortfolioViewerDataGrid.DataContext = PortfolioManager.GetPortfolioData();
            PortfolioValueDisplay.UpdateCryptoValueDisplay();
        }

        private void DisplayCurrencyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PortfolioValueDisplay.DisplayCurrency = DisplayCurrencyComboBox.SelectedValue.ToString()!;
            PortfolioViewer.PortfolioViewerDataGrid.DataContext = PortfolioManager.GetPortfolioData();
            PortfolioValueDisplay.UpdateCurrencyValueDisplay();

        }
    }
}
