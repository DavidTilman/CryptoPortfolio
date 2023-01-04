using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace CryptoPortfolio
{
    internal class PortfolioCoin
    {
        public string Symbol { get; set; }
        public double Quantity { get; set; }
        public string Currency { get; set; }
        public double PricePerCoin { get; set; }
        public PortfolioCoin() { }
        public PortfolioCoin(string symbol, double quantity, string currency, double price_per_coin)
        {
            Symbol = symbol;
            Quantity = quantity;
            Currency = currency;
            PricePerCoin = price_per_coin;
        }

        public static List<string> GetComparableData(PortfolioCoin coin)=> new () { coin.Symbol, coin.PricePerCoin.ToString(), coin.Currency };
    }

    internal class PortfolioCoinData : PortfolioCoin
    {
        public double ValueAtBuy { get; set; }
        public double CurrentValue { get; set; }
        public string PercentChange { get; set; }

        public PortfolioCoinData() { }
        public PortfolioCoinData(string symbol, double quantity, string currency, double price, double current_value, string percent_change) : base(symbol, quantity, currency, price)
        {
            CurrentValue = current_value;
            PercentChange = percent_change;
        }
    }
     
    internal class PortfolioManager
    {
        public static Dictionary<string, List<string>> GetCoinsContained()
        {
            List<PortfolioCoin> current_portfolio = JsonSerializer.Deserialize<List<PortfolioCoin>>(File.ReadAllText(AppData.current_portfolio_path))!;
            HashSet<string> symbols = new(AppData.display_cryptos);
            HashSet<string> currencies = new (AppData.display_currencies.Concat(AppData.display_cryptos));
            foreach (PortfolioCoin portfolioCoin in current_portfolio)
            {
                symbols.Add (portfolioCoin.Symbol);
                currencies.Add (portfolioCoin.Currency);
            }
            return new Dictionary<string, List<string>>()
            {
                {"Symbols", symbols.ToList()},
                {"Currencies", currencies.ToList()}
            };
        }
        public static void Add (PortfolioCoin coin)
        {
            List<PortfolioCoin> current_portfolio = JsonSerializer.Deserialize<List<PortfolioCoin>>(File.ReadAllText(AppData.current_portfolio_path))!;
            List<string> coin_data = PortfolioCoin.GetComparableData(coin);
            foreach (PortfolioCoin portfolioCoin in current_portfolio)
            {
                List<string> portfolio_coin_data = PortfolioCoin.GetComparableData(portfolioCoin);
                if (coin_data.SequenceEqual(portfolio_coin_data))
                {
                    portfolioCoin.Quantity += coin.Quantity;
                    File.WriteAllText(AppData.current_portfolio_path, JsonSerializer.Serialize(current_portfolio, new JsonSerializerOptions { WriteIndented = true }));
                    return;
                }
            }
            current_portfolio.Add(coin);
            File.WriteAllText(AppData.current_portfolio_path, JsonSerializer.Serialize(current_portfolio, new JsonSerializerOptions { WriteIndented = true }));
            ApiServicer.CallToJsonFile(ApiQueryBuilder.GetPriceQuery(GetCoinsContained()["Symbols"], GetCoinsContained()["Currencies"]), AppData.current_api_call_path);
            CrytpoPriceApiResponse res = new(JsonServicer.ReadFile(AppData.current_api_call_path), GetCoinsContained()["Symbols"], GetCoinsContained()["Currencies"]);
            if(res.HasError)
            {
                MessageBox.Show(res.ErrorMessage);
                current_portfolio.Remove(coin);
                File.WriteAllText(AppData.current_portfolio_path, JsonSerializer.Serialize(current_portfolio, new JsonSerializerOptions { WriteIndented = true }));
                MessageBox.Show($"Coin: {coin.Symbol} was not added to portfolio.");
            }
            else
            {
                MessageBox.Show($"Coin: {coin.Symbol} added to portfolio successfully.");
            }
        }
        public static string Get()
        {
            List<PortfolioCoin> current_portfolio = JsonSerializer.Deserialize<List<PortfolioCoin>>(File.ReadAllText(AppData.current_portfolio_path))!;
            string data_string = "";
            foreach (PortfolioCoin portfolioCoin in current_portfolio)
            {
                data_string += $"{portfolioCoin.Symbol}: {portfolioCoin.Quantity} @ {portfolioCoin.PricePerCoin} {portfolioCoin.Currency}";
            }
            return data_string;
        }

        public static double GetValueCurrent(string currency_symbol)
        {
            double portfolio_value = 0;

            CrytpoPriceApiResponse current_prices = new(JsonServicer.ReadFile(AppData.current_api_call_path), GetCoinsContained()["Symbols"], GetCoinsContained()["Currencies"]);

            if (current_prices.HasError)
            {
                MessageBox.Show("Error: " + current_prices.ErrorMessage);
                return 0;
            }

            List<PortfolioCoin> current_portfolio = JsonSerializer.Deserialize<List<PortfolioCoin>>(File.ReadAllText(AppData.current_portfolio_path))!;
            foreach (PortfolioCoin coin in current_portfolio)
            {
                double coin_current_price = double.Parse(current_prices.currency_data[coin.Symbol][currency_symbol]);
                double current_coin_value = Math.Round(coin_current_price * coin.Quantity, 8);
                portfolio_value+= current_coin_value;
            }
            return Math.Round(portfolio_value, 8);
        }

        public static List<PortfolioCoinData>  GetPortfolioData()
        {
            List<PortfolioCoin> current_portfolio = JsonSerializer.Deserialize<List<PortfolioCoin>>(File.ReadAllText(AppData.current_portfolio_path))!;
            return (from PortfolioCoin coin in current_portfolio select GetCoinData(coin)).ToList();
        }

        public static PortfolioCoinData GetCoinData(PortfolioCoin coin)
        {
            CrytpoPriceApiResponse current_prices = new(JsonServicer.ReadFile(AppData.current_api_call_path), GetCoinsContained()["Symbols"], GetCoinsContained()["Currencies"]);

            if (current_prices.HasError)
            {
                MessageBox.Show("Error: " + current_prices.ErrorMessage);
                return new PortfolioCoinData();
            }
            double value_at_buy = coin.PricePerCoin * coin.Quantity;
            double value_now = double.Parse(current_prices.currency_data[coin.Symbol][coin.Currency]) * coin.Quantity;
            return new PortfolioCoinData()
            {
                Symbol = coin.Symbol,
                Quantity = coin.Quantity,
                Currency = coin.Currency,
                PricePerCoin = coin.PricePerCoin,
                ValueAtBuy = Math.Round(coin.PricePerCoin * coin.Quantity, 8),
                CurrentValue = Math.Round(value_now, 8),
                PercentChange = Math.Round(((value_now - value_at_buy) / value_at_buy)*100, 2).ToString() + "%"
            };
        }
    }
}
