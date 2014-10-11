using Bash.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bash.Common.Data
{
    public interface ICachedBashClient : IBashClient
    {
        Task<BashCollection> GetQuotesAsync(string order, int number, int page, bool forceReload);

        Task<BashComments> GetCommentsAsync(int id, bool foreceReload);

        void UpdateCache(BashCollection data);
    }
}
