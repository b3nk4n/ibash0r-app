using Bash.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Bash.App.ViewModels
{
    interface ICategoryViewModel
    {
        Task<bool> LoadQuotesAsync(string order);

        Task<bool> SearchQuotesAsync(string term);

        BashData CurrentBashData { get; }

        NavigationService NavigationService { set; }

        ICommand NextCommand { get; }

        ICommand PreviousCommand { get; }

        ICommand RatePositiveCommand { get; }

        ICommand RateNegativeCommand { get; }
    }
}
