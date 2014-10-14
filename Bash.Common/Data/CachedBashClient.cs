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
        private IBashClient _bashClient;

        private const string BASH_CACHE_FORMAT = "cache_{0}.data";

        private const string SEARCH_PREFIX = "search_";

        private const string COMMENTS_PREFIX = "comments_";

        private const string WARTE_KEY = "WARTE";

        private string _lastLoadedOrder;

        public const double LIFE_TIME_DAYS_LONG = 365.0;
        public const double LIFE_TIME_DAYS_DEFAULT = 30.0;
        public const double LIFE_TIME_DAYS_SHORT = 1.0;

        /// <summary>
        /// The stored deadlines when the quotes cache data is invalid.
        /// IDs: favorites, new, best, random
        /// </summary>
        private StoredObject<Dictionary<string, DateTime>> _quotesCacheDeadlines = new StoredObject<Dictionary<string, DateTime>>("__quotesDeadlines", new Dictionary<string, DateTime>());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bashClient"></param>

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
            DateTime deadline = DateTime.MaxValue;

            // check deadline/cache lifetime
            if (_quotesCacheDeadlines.Value.ContainsKey(order))
            {
                deadline = _quotesCacheDeadlines.Value[order];
            }

            // load locally
            if (!forceReload && StorageHelper.FileExists(cacheFileName))
            {
                result = StorageHelper.LoadSerializedFile<BashCollection>(cacheFileName);
            }

            // load from web
            if (result == null ||
               (result != null && result.Contents.Data.Count < number) || // ensure there is at least 'number' of quotes, which could be lower (50), caused by the lockscreen pre-load.
               (DateTime.Now < deadline && result.Contents.Data.Count < number))
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

        public async Task<BashCollection> GetQueryAsync(string term, int number, int page)
        {
            var searchKey = SEARCH_PREFIX + term;
            if (PhoneStateHelper.ValueExists(searchKey))
            {
                return PhoneStateHelper.LoadValue<BashCollection>(searchKey);
            }

            var result = await _bashClient.GetQueryAsync(term, number, page);
            if (result != null)
            {
                PhoneStateHelper.SaveValue(searchKey, result);
            }
            return result;
        }

        public Task<BashComments> GetCommentsAsync(int id)
        {
            return GetCommentsAsync(id, false);
        }

        public async Task<BashComments> GetCommentsAsync(int id, bool forceReload)
        {
            var commentsKey = COMMENTS_PREFIX + id;
            if (!forceReload && PhoneStateHelper.ValueExists(commentsKey))
            {
                return PhoneStateHelper.LoadValue<BashComments>(commentsKey);
            }

            var result = await _bashClient.GetCommentsAsync(id);
            if (result != null)
            {
                PhoneStateHelper.SaveValue(commentsKey, result);
            }
            return result;
        }

        public Task<bool> RateAsync(int id, string type)
        {
            return _bashClient.RateAsync(id, type);
        }

        public Task<BashCollection> GetWarteAsync()
        {
            return GetWarteAsync(false);
        }

        public async Task<BashCollection> GetWarteAsync(bool forceReload)
        {
            if (!forceReload && PhoneStateHelper.ValueExists(WARTE_KEY))
            {
                return PhoneStateHelper.LoadValue<BashCollection>(WARTE_KEY);
            }

            var result = await _bashClient.GetWarteAsync();
            if (result != null)
            {
                PhoneStateHelper.SaveValue(WARTE_KEY, result);
            }
            return result;
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
    }
}
