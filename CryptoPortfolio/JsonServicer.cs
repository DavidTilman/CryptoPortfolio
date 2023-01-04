using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CryptoPortfolio
{
    internal class JsonServicer
    {
        public static JsonObject? ReadFile(string filename)
        {
            try
            {
                return JsonNode.Parse(File.ReadAllText(filename))?.AsObject();
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
    }
}
