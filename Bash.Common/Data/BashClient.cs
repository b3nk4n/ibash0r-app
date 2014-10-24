using Bash.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bash.Common.Data
{
    public class BashClient : IBashClient
    {
        #region Members

        private const string BASE_URI = "http://www.ibash.de/iphone";

        private const string PATH_QUOTE = "/quote.php";
        private const string PATH_QUOTES = "/quotes2.php";
        private const string PATH_QUERY = "/query.php";
        private const string PATH_COMMENTS = "/comments.php";
        private const string PATH_RATE = "/rate.php";
        private const string PATH_WARTE = "/warte.php";

        private HttpClient _httpClient = new HttpClient();

        #endregion

        #region Constructors

        public BashClient()
        {  
        }

        #endregion

        #region Public Methods

        public async Task<BashCollection> GetQuotesAsync(string order, int number, int page)
        {
            string uriString = String.Format("{0}{1}?{2}={3}&{4}={5}&{6}={7}",
                BASE_URI, PATH_QUOTES,
                AppConstants.PARAM_ORDER, order,
                AppConstants.PARAM_NUMBER, number,
                AppConstants.PARAM_PAGE, page);
            var response = await _httpClient.GetAsync(uriString);
            
            if (response.IsSuccessStatusCode)
            {
                var encodedData = await ReadEncodedContentAsync(response);
                return JsonConvert.DeserializeObject<BashCollection>(encodedData);
            }

            return null;
        }

        public async Task<BashCollection> GetQueryAsync(string term, int number, int page)
        {
            // TODO: escape "term" string?

            string uriString = String.Format("{0}{1}?{2}={3}&{4}={5}&{6}={7}",
                BASE_URI, PATH_QUERY,
                AppConstants.PARAM_TERM, term,
                AppConstants.PARAM_NUMBER, number,
                AppConstants.PARAM_PAGE, page);
            var response = await _httpClient.GetAsync(uriString);

            if (response.IsSuccessStatusCode)
            {
                var encodedData = await ReadEncodedContentAsync(response);
                return JsonConvert.DeserializeObject<BashCollection>(encodedData);
            }

            return null;
        }


        public async Task<BashComments> GetCommentsAsync(int id)
        {
            string uriString = String.Format("{0}{1}?{2}={3}",
                BASE_URI, PATH_COMMENTS,
                AppConstants.PARAM_ID, id);
            var response = await _httpClient.GetAsync(uriString);

            if (response.IsSuccessStatusCode)
            {
                var encodedData = await ReadEncodedContentAsync(response);
                var data = JsonConvert.DeserializeObject<BashComments>(encodedData);
                
                // trim to 50 items
                if (data.Comments.Count > 50)
                {
                    data.Comments.RemoveRange(50, data.Comments.Count - 50);
                }

                for (int i = 0; i < data.Comments.Count; ++i)
                {
                    data.Comments[i].IndexPosition = i;
                }
                return data;
            }

            return null;
        }

        public async Task<bool> RateAsync(int id, string type)
        {
            string uriString = String.Format("{0}{1}?{2}={3}&{4}={5}",
                BASE_URI, PATH_RATE,
                AppConstants.PARAM_ID, id,
                AppConstants.PARAM_TYPE, type);

            var formData = new FormUrlEncodedContent(new[] 
            {
                new KeyValuePair<string, string>(AppConstants.PARAM_ID, id.ToString()),
                new KeyValuePair<string, string>(AppConstants.PARAM_TYPE, type)
            });

            var response = await _httpClient.PostAsync(uriString, formData);

            if (response.IsSuccessStatusCode)
            {
                var encodedData = await ReadEncodedContentAsync(response);
                return true;
            }

            return false;
        }

        public async Task<BashCollection> GetWarteAsync()
        {
            string uriString = String.Format("{0}{1}",
                BASE_URI, PATH_WARTE);
            var response = await _httpClient.GetAsync(uriString);

            if (response.IsSuccessStatusCode)
            {
                var encodedData = await ReadEncodedContentAsync(response);
                return JsonConvert.DeserializeObject<BashCollection>(encodedData);
            }

            return null;
        }

        #endregion

        #region Private Methods

        private static async Task<string> ReadEncodedContentAsync(HttpResponseMessage response)
        {
            var data = await response.Content.ReadAsByteArrayAsync();
            string encodedString = Encoding.GetEncoding("iso-8859-1").GetString(data, 0, data.Length);
            return encodedString;
        }

        #endregion
    }
}
