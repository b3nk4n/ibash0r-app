using Bash.Common.Data;
using Bash.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bash.App.Data
{
    public interface IFullyCachedBashClient : ICachedBashClient
    {
        Task<BashComments> GetCommentsAsync(int id, bool foreceReload);

        Task<BashCollection> GetQueryAsync(string term, int number, int page, bool forceReload);

        Task<BashCollection> GetWarteAsync(bool foreceReload);
    }
}
