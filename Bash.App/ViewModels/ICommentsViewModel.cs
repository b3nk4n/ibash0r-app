using Bash.Common.Models;
using System.Threading.Tasks;

namespace Bash.App.ViewModels
{
    public interface ICommentsViewModel
    {
        Task<bool> LoadCommentsAsync(int id);

        void Reset();

        BashComments BashComments { get; }

        bool IsBusy { get; }
    }
}
