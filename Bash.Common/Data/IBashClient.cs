using Bash.Common.Models;
using System.Threading.Tasks;

namespace Bash.Common.Data
{
    public interface IBashClient
    {
        Task<BashCollection> GetQuotesAsync(string order, int number, int page);

        Task<BashCollection> GetQueryAsync(string term, int number, int page);

        Task<BashComments> GetCommentsAsync(int id);

        Task<bool> RateAsync(int id, string type);

        Task<BashCollection> GetWarteAsync();
    }
}
