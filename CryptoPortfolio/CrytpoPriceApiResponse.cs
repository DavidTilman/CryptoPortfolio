using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CryptoPortfolio
{
    internal class CrytpoPriceApiResponse
    {
        public Dictionary<string, Dictionary<string, string>> currency_data = new();
        public bool HasError { get { return has_error; } }
        private readonly bool has_error = false;

        public string ErrorMessage { get { return error_message; } }
        private readonly string error_message = "";

        public CrytpoPriceApiResponse(JsonObject? json, IEnumerable<string> symbols, IEnumerable<string> currencies) 
        { 
            if (json is null || json["Response"]?.ToString() == "Error") { 
                has_error = true;
                if (json is not null && json["Message"] is not null) 
                { 
                    error_message = json["Message"]!.ToString();
                }
                return; 
            }
           foreach (string symbol in symbols)
            {
                if (!json.ContainsKey(symbol))
                {
                    has_error = true;
                    error_message = $"API call did not return requested symbol: {symbol}";
                    return;
                }
                Dictionary<string, string> symbol_data = new();
                foreach (string currency in currencies)
                {
                    if (!json[symbol]!.AsObject().ContainsKey(currency))
                    {
                        has_error = true;
                        error_message = $"API call did not return requested currency: {currency}";
                        return;
                    }
                    symbol_data.Add(currency, json[symbol]![currency]!.ToString());
                }
                currency_data.Add(symbol, symbol_data);
            }
        }
    }
}
