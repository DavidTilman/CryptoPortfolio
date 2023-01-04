using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CryptoPortfolio
{
    internal class ApiServicer
    {
        private static HttpResponseMessage MakeApiCall(string query)
        {
            return new HttpClient().GetAsync(query).Result;
        }
        public static void CallToJsonFile(string query, string filename)
        {
            HttpResponseMessage response = MakeApiCall(query);
            File.WriteAllText(filename, response.Content.ReadAsStringAsync().Result);
        }
    }
}
