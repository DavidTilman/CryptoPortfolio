using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoPortfolio;
internal class ApiQueryBuilder
{
    const string api_price_url = "https://min-api.cryptocompare.com/data/";

    const string app_name_query = "extraParams";
    const string api_app_name = "PortfolioTracker";

    const string multi_price = "pricemulti";

    const string multi_symbol_query = "fsyms";

    const string multi_currency_query = "tsyms";

    const string api_key_query = "api_key";
    const string api_key = "140d708907dcdd025956851bbf24fec3d5521727682d5fbde67dea587235cad7";

    public static string GetPriceQuery(string symbol, string currency) =>
        $"{api_price_url}{multi_price}?" +
        $"{Query(app_name_query, api_app_name)}" +
        $"{Query(multi_symbol_query, symbol)}" +
        $"{Query(multi_currency_query, currency)}" +
        $"{Query(api_key_query, api_key)}";

    public static string GetPriceQuery(IEnumerable<string> symbols, IEnumerable<string> currencies) =>
        $"{api_price_url}{multi_price}?" +
        $"{Query(app_name_query, api_app_name)}" +
        $"{Query(multi_symbol_query, string.Join(",", symbols))}" +
        $"{Query(multi_currency_query, string.Join(",", currencies))}" +
        $"{Query(api_key_query, api_key)}";

    private static string Query(string property, string value) => $"&{property}={value}";
}
