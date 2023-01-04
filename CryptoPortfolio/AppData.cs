using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPortfolio
{
    internal static class AppData
    {
        public static readonly int api_calls_per_month = 99_000;
        public static readonly TimeSpan UpdateRate = new TimeSpan(31, 0, 0, 0) / api_calls_per_month;
        public static readonly string portfolio_last_data_path = "portfolio_last_data.json";
        public static readonly string current_api_call_path = "current_api_call.json";
        public static readonly string current_portfolio_path = "current_portfolio.json";
        public static readonly HashSet<string> display_cryptos = new HashSet<string>() { "BTC", "ETH", "XRP"};
        public static readonly HashSet<string> display_currencies = new HashSet<string>() { "USD", "GBP", "EUR"};
    }
}
