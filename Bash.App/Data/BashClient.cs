using Bash.App.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bash.App.Data
{
    public class BashClient : IBashClient
    {
        private const string BASE_URI = "http://www.ibash.de/iphone";

        private const string PATH_QUOTE = "/quote.php";
        private const string PATH_QUOTES = "/quotes2.php";
        private const string PATH_QUERY = "/query.php";
        private const string PATH_COMMENTS = "/comments.php";
        private const string PATH_RATE = "/rate.php";

        public const string PARAM_TERM = "term";
        public const string PARAM_TYPE = "type";
        public const string PARAM_ID = "id";
        public const string PARAM_ORDER = "order";
        public const string PARAM_NUMBER = "number";
        public const string PARAM_PAGE = "page";

        public const string ORDER_VALUE_NEW = "new";
        public const string ORDER_VALUE_BEST = "best";
        public const string ORDER_VALUE_RANDOM = "random";

        public const string TYPE_VALUE_POS = "pos";
        public const string TYPE_VALUE_NEG = "neg";

        private HttpClient _httpClient = new HttpClient();

        public BashClient()
        {  
        }

        public async Task<BashCollection> GetQuotesAsync(string order, int number, int page)
        {
            string uriString = String.Format("{0}{1}?{2}={3}&{4}={5}&{6}={7}",
                BASE_URI, PATH_QUOTES,
                PARAM_ORDER, order,
                PARAM_NUMBER, number,
                PARAM_PAGE, page);
            var response = await _httpClient.GetAsync(uriString);
            
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsByteArrayAsync();

                string encodedString = Encoding.GetEncoding("iso-8859-1").GetString(data, 0, data.Length);

                return JsonConvert.DeserializeObject<BashCollection>(encodedString);
            }

            return null;
        }

        public async Task<BashCollection> GetQueryAsync(string term, int number, int page)
        {
            // TODO: escape "term" string?

            string uriString = String.Format("{0}{1}?{2}={3}&{4}={5}&{6}={7}",
                BASE_URI, PATH_QUOTES,
                PARAM_TERM, term,
                PARAM_NUMBER, number,
                PARAM_PAGE, page);
            var response = await _httpClient.GetAsync(uriString);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<BashCollection>(data);
            }

            return null;
        }


        public async Task<BashComments> GetCommentsAsync(string id)
        {
            string uriString = String.Format("{0}{1}?{2}={3}",
                BASE_URI, PATH_COMMENTS,
                PARAM_ID, id);
            var response = await _httpClient.GetAsync(uriString);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<BashComments>(data);
            }

            return null;
        }

        public async Task<string> RateAsync(string id, string type)
        {
            string uriString = String.Format("{0}{1}?{2}={3}&{4}={5}",
                BASE_URI, PATH_RATE,
                PARAM_ID, id,
                PARAM_TYPE, type);
            var response = await _httpClient.GetAsync(uriString);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                return data;
            }

            return null;
        }
    }
}
