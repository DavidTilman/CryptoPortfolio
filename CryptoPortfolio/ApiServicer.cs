using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;

namespace CryptoPortfolio
{
    internal class ApiServicer
    {
        private static HttpResponseMessage? MakeApiCall(string query)
        {
            try
            {
                return new HttpClient().GetAsync(query).Result;
            }
            catch (AggregateException)
            {
                return null;
            }
        }
        public static void CallToJsonFile(string query, string filename)
        {
            HttpResponseMessage? response = MakeApiCall(query);
            if (response is null)
            {
                MessageBox.Show("Network Error.");
                return;
            }
            File.WriteAllText(filename, response.Content.ReadAsStringAsync().Result);
            
        }
    }
}
