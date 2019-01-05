using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Spell.Bing.API
{
    public class Analyse
    {
        static string host = "https://api.cognitive.microsoft.com";
        static string link = "/bing/v7.0/spellcheck?";
        static string param = "mkt=en-US&mode=proof";
        static string key = "ENTER KEY HERE";
        public async static Task<string> SpellCheck(string textInput)
        {
            HttpClient client = new HttpClient();
            string clientId;
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            HttpResponseMessage response = new HttpResponseMessage();
            string uri = host + link + param;

            List<KeyValuePair<string, string>> valuesPair = new List<KeyValuePair<string, string>>();
            valuesPair.Add(new KeyValuePair<string, string>("text", textInput));

            using (FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(valuesPair))
            {
                formUrlEncodedContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                response = await client.PostAsync(uri, formUrlEncodedContent);
            }
            if (response.Headers.TryGetValues("X-MSEdge-ClientID", out IEnumerable<string> header_values))            
                clientId = header_values.Cast<string>().First();           
            

            string contentString = await response.Content.ReadAsStringAsync();
            return ToJSON(contentString);

        }
        static string ToJSON(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {

                json = json.Replace(Environment.NewLine, "").Replace("\t", "");

                StringBuilder sb = new StringBuilder();
                bool quote = false;
                bool ignore = false;
                char last = ' ';
                int offset = 0;
                int indentLength = 3;

                foreach (char item in json)
                {
                    switch (item)
                    {
                        case '"':
                            if (!ignore) quote = !quote;
                            break;
                        case '\\':
                            if (quote && last != '\\') ignore = true;
                            break;
                    }

                    if (quote)
                    {
                        sb.Append(item);
                        if (last == '\\' && ignore) ignore = false;
                    }
                    else
                    {
                        switch (item)
                        {
                            case '{':
                            case '[':
                                sb.Append(item);
                                sb.Append(Environment.NewLine);
                                sb.Append(new string(' ', ++offset * indentLength));
                                break;
                            case '}':
                            case ']':
                                sb.Append(Environment.NewLine);
                                sb.Append(new string(' ', --offset * indentLength));
                                sb.Append(item);
                                break;
                            case ',':
                                sb.Append(item);
                                sb.Append(Environment.NewLine);
                                sb.Append(new string(' ', offset * indentLength));
                                break;
                            case ':':
                                sb.Append(item);
                                sb.Append(' ');
                                break;
                            default:
                                if (quote || item != ' ') sb.Append(item);
                                break;
                        }
                    }
                    last = item;
                }

                return sb.ToString().Trim();
            }
            else
                return string.Empty;
        }

    }
}
