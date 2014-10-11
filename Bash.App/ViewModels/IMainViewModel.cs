using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Bash.App.ViewModels
{
    public interface IMainViewModel
    {
        string SearchTerm { get; set; }

        NavigationService NavigationService { set; }

        ICommand SearchCommand { get; }

        ICommand SetLockScreenCommand { get; }

        ICommand BackupCommand { get; }
    }
}
