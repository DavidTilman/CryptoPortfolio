using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.IO;

namespace CryptoPortfolio
{
    /// <summary>
    /// Interaction logic for PortfolioValueDisplay.xaml
    /// </summary>
    public partial class PortfolioValueDisplay : UserControl
    {
        [Category("Data")]
        public string DisplaySymbol { get; set; }
        public string DisplayCurrency { get; set; }
        public PortfolioValueDisplay()
        {
            InitializeComponent();
        }
        public void UpdateCryptoValueDisplay()
        {
            double portfolio_value = PortfolioManager.GetValueCurrent(DisplaySymbol.ToString());
            double last_portfolio_value = double.Parse(JsonServicer.ReadFile(AppData.portfolio_last_data_path)!["LastCrypto"]!.ToString());
            CryptoValueDisplayLabel.Content = 
                $"{portfolio_value} {DisplaySymbol} " +
                $"{GetChangeChar(portfolio_value, last_portfolio_value)}";
        }
        public void UpdateCurrencyValueDisplay()
        {
            double portfolio_value = PortfolioManager.GetValueCurrent(DisplayCurrency.ToString());
            double last_portfolio_value = double.Parse(JsonServicer.ReadFile(AppData.portfolio_last_data_path)!["LastCurrency"]!.ToString());
            CurrencyValueDisplayLabel.Content =
                $"{Math.Round(portfolio_value, 2)} {DisplayCurrency} " +
                $"{GetChangeChar(portfolio_value, last_portfolio_value)}";
        }

        private static char GetChangeChar(double current, double last)
        {
            return (double)(current - last) switch
            {
                0 => '-',
                > 0 => '▲',
                < 0 => '▼',
                _ => '-',
            };
        }
    }
}
