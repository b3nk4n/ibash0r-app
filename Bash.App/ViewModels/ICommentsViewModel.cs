using Bash.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bash.App.ViewModels
{
    public interface ICommentsViewModel
    {
        Task<bool> LoadCommentsAsync(int id);

        BashComments BashComments { get; }
    }
}
