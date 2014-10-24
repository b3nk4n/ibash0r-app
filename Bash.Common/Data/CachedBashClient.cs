using Bash.Common.Models;
using PhoneKit.Framework.Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bash.Common.Data
{
    public class CachedBashClient : ICachedBashClient
    {
        private readonly IBashClient _bashClient;

        private const string BASH_CACHE_FORMAT = "cache_{0}.data";

        private string _lastLoadedOrder;

        public const double LIFE_TIME_DAYS_LONG = 365.0;
        public const double LIFE_TIME_DAYS_DEFAULT = 30.0;
        public const double LIFE_TIME_DAYS_SHORT = 1.0;

        /// <summary>
        /// The stored deadlines when the quotes cache data is invalid.
        /// IDs: favorites, new, best, random
        /// </summary>
        private StoredObject<Dictionary<string, DateTime>> _quotesCacheDeadlines = new StoredObject<Dictionary<string, DateTime>>("__quotesDeadlines", new Dictionary<string, DateTime>());

        public CachedBashClient(IBashClient bashClient)
        {
            _bashClient = bashClient;
        }

        public Task<BashCollection> GetQuotesAsync(string order, int number, int page)
        {
            return GetQuotesAsync(order, number, page, LIFE_TIME_DAYS_DEFAULT, false);
        }

        public async Task<BashCollection> GetQuotesAsync(string order, int number, int page, double lifeTimeDays, bool forceReload)
        {
            BashCollection result = null;
            string cacheFileName = string.Format(BASH_CACHE_FORMAT, order);
            _lastLoadedOrder = order;
            DateTime deadline = DateTime.MinValue;

            // check deadline/cache lifetime
            if (_quotesCacheDeadlines.Value.ContainsKey(order))
            {
                deadline = _quotesCacheDeadlines.Value[order];
            }

            // load locally
            if (!forceReload && StorageHelper.FileExists(cacheFileName) && DateTime.Now < deadline)
            {
                result = StorageHelper.LoadSerializedFile<BashCollection>(cacheFileName);
            }

            // load from web
            if (result == null || result.Contents.Data.Count < number)
            {
                result = await _bashClient.GetQuotesAsync(order, number, page);
                if (result != null)
                {
                    StorageHelper.SaveAsSerializedFile<BashCollection>(cacheFileName, result);
                    
                    // update deadline/cache lifetime for the data
                    var newDeadline = DateTime.Now.AddDays(lifeTimeDays);
                    if (_quotesCacheDeadlines.Value.ContainsKey(order))
                    {
                        _quotesCacheDeadlines.Value[order] = newDeadline;
                    }
                    else
                    {
                        _quotesCacheDeadlines.Value.Add(order, newDeadline);
                    }
                }
            }

            return result;
        }

        public Task<bool> RateAsync(int id, string type)
        {
            return _bashClient.RateAsync(id, type);
        }

        public void UpdateCache(BashCollection data)
        {
            if (_lastLoadedOrder != null &&
                data != null)
            {
                string cacheFileName = string.Format(BASH_CACHE_FORMAT, _lastLoadedOrder);
                StorageHelper.SaveAsSerializedFile<BashCollection>(cacheFileName, data);
            }
        }


        public Task<BashCollection> GetQueryAsync(string term, int number, int page)
        {
            return _bashClient.GetQueryAsync(term, number, page);
        }

        public Task<BashComments> GetCommentsAsync(int id)
        {
            return _bashClient.GetCommentsAsync(id);
        }

        public Task<BashCollection> GetWarteAsync()
        {
            return _bashClient.GetWarteAsync();
        }
    }
}
