using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CryptoPortfolio
{
    /// <summary>
    /// Interaction logic for PortfolioViewer.xaml
    /// </summary>
    public partial class PortfolioViewer : UserControl
    {
        public PortfolioViewer()
        {
            InitializeComponent();
            PortfolioViewerDataGrid.DataContext = PortfolioManager.GetPortfolioData();
        }

        private void PortfolioViewerDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            List<string> ordered_column_headers = new() { "Symbol", "Quantity", "Currency", "Price", "Value at Buy", "Current Value", "Percent Change" };
            int index = ordered_column_headers.IndexOf(e.Column.Header.ToString()!);
            Console.WriteLine(index);
            if (index == -1)
            {
                e.Cancel = true;
                return;
            }
            e.Column.DisplayIndex = index;
        }
    }
}
