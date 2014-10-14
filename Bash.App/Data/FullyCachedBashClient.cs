using Bash.Common.Data;
using Bash.Common.Models;
using PhoneKit.Framework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bash.App.Data
{
    public class FullyCachedBashClient : IFullyCachedBashClient
    {
        private readonly ICachedBashClient _bashClient;

        private const string SEARCH_PREFIX = "search_";

        private const string COMMENTS_PREFIX = "comments_";

        private const string WARTE_KEY = "WARTE";

        public FullyCachedBashClient(ICachedBashClient bashClient)
        {
            _bashClient = bashClient;
        }

        public Task<BashCollection> GetQueryAsync(string term, int number, int page)
        {
            return GetQueryAsync(term, number, page, false);
        }

        public async Task<BashCollection> GetQueryAsync(string term, int number, int page, bool forceRealod)
        {
            var searchKey = SEARCH_PREFIX + term;
            if (!forceRealod && PhoneStateHelper.ValueExists(searchKey))
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

        public Task<BashCollection> GetQuotesAsync(string order, int number, int page, double lifeTimeDays, bool forceReload)
        {
            return _bashClient.GetQuotesAsync(order, number, page, lifeTimeDays, forceReload);
        }

        public void UpdateCache(BashCollection data)
        {
            _bashClient.UpdateCache(data);
        }

        public Task<BashCollection> GetQuotesAsync(string order, int number, int page)
        {
            return _bashClient.GetQuotesAsync(order, number, page);
        }

        public Task<bool> RateAsync(int id, string type)
        {
            return _bashClient.RateAsync(id, type);
        }
    }
}
