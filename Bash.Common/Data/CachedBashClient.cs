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

        private string _lastLoadedOrder;

        private Dictionary<int, BashComments> _commentsMemoryCache = new Dictionary<int, BashComments>();

        public CachedBashClient(IBashClient bashClient)
        {
            _bashClient = bashClient;
        }

        public Task<BashCollection> GetQuotesAsync(string order, int number, int page)
        {
            return GetQuotesAsync(order, number, page, false);
        }

        public async Task<BashCollection> GetQuotesAsync(string order, int number, int page, bool forceReload)
        {
            BashCollection result = null;
            string cacheFileName = string.Format(BASH_CACHE_FORMAT, order);
            _lastLoadedOrder = order;

            //if (forceReload || !StorageHelper.FileExists(cacheFileName))
            //{
            //    result = await _bashClient.GetQuotesAsync(order, number, page);
            //    if (result != null)
            //    {
            //        StorageHelper.SaveAsSerializedFile<BashCollection>(cacheFileName, result);
            //    }
            //}
            //else
            //{
            //    result = StorageHelper.LoadSerializedFile<BashCollection>(cacheFileName);
            //}

            ///------
            var cacheFileExists = StorageHelper.FileExists(cacheFileName);
            if (!forceReload && cacheFileExists)
            {
                result = StorageHelper.LoadSerializedFile<BashCollection>(cacheFileName);
            }
            
            if (result == null || forceReload || !cacheFileExists || (cacheFileExists && result.Contents.Data.Count < number)) // ensure there is at least 'number' of quotes, which could be lower (15), caused by the lockscreen pre-load.
            {
                result = await _bashClient.GetQuotesAsync(order, number, page);
                if (result != null)
                {
                    StorageHelper.SaveAsSerializedFile<BashCollection>(cacheFileName, result);
                }
            }

            return result;
        }

        public Task<BashCollection> GetQueryAsync(string term, int number, int page)
        {
            return _bashClient.GetQueryAsync(term, number, page);
        }

        public Task<BashComments> GetCommentsAsync(int id)
        {
            return GetCommentsAsync(id, false);
        }

        public async Task<BashComments> GetCommentsAsync(int id, bool forceReload)
        {
            BashComments result;

            if (forceReload || !_commentsMemoryCache.ContainsKey(id))
            {
                result = await _bashClient.GetCommentsAsync(id);

                if (result != null)
                {
                    _commentsMemoryCache.Add(id, result);
                }
            }
            else
            {
                result = _commentsMemoryCache[id];
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
    }
}
