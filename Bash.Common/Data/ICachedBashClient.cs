using Bash.Common.Models;
using System.Threading.Tasks;

namespace Bash.Common.Data
{
    public interface ICachedBashClient : IBashClient
    {
        Task<BashCollection> GetQuotesAsync(string order, int number, int page, double lifeTimeDays, bool forceReload);

        void UpdateCache(BashCollection data);
    }
}
