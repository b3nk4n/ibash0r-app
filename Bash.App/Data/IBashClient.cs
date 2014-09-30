using Bash.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bash.App.Data
{
    public interface IBashClient
    {
        Task<BashCollection> GetQuotesAsync(string order, int number, int page);

        Task<BashCollection> GetQueryAsync(string term, int number, int page);

        Task<BashComments> GetCommentsAsync(string id);

        Task<string> RateAsync(string id, string type);
    }
}
