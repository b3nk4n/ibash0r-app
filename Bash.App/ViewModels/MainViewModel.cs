using Bash.Common.Controls;
using Bash.Common.Data;
using PhoneKit.Framework.Core.Graphics;
using PhoneKit.Framework.Core.LockScreen;
using PhoneKit.Framework.Core.MVVM;
using PhoneKit.Framework.Core.Storage;
using PhoneKit.Framework.InAppPurchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Ninject;
using Bash.Common;
using PhoneKit.Framework.Tasks;
using Microsoft.Phone.Scheduler;
using Bash.App.Helpers;

namespace Bash.App.ViewModels
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        #region Members

        private string _searchTerm;

        private NavigationService _navigationService;

        private DelegateCommand _searchCommand;
        private DelegateCommand _setLockScreenCommand;
        private DelegateCommand _backupCommand;

        private ICachedBashClient _bashClient;

        private bool _isBusy;

        #endregion

        #region Constructors

        public MainViewModel()
        {
            InitializeCommands();
            _bashClient = App.Injector.Get<ICachedBashClient>();
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            _searchCommand = new DelegateCommand(() =>
            {
                if (CanSearch)
                {
                    var uriString = "/Pages/CategoryPage.xaml?term=" + SearchTerm;
                    NavigationService.Navigate(new Uri(uriString, UriKind.Relative));
                }
                
            }, () =>
            {
                return true;
            });

            _setLockScreenCommand = new DelegateCommand(async () =>
            {
#if !DEBUG
                if (!InAppPurchaseHelper.IsProductActive(AppConstants.IAP_AWESOME_EDITION))
                {
                    NavigationService.Navigate(new Uri("/Pages/InAppStorePage.xaml", UriKind.Relative));
                }
                else
#endif
                {
                    if (!LockScreenHelper.HasAccess())
                    {
                        if (await LockScreenHelper.VerifyAccessAsync())
                        {
                            
                        }

                        IsBusy = true;
                        UpdateBackgroundTask();
                        IsBusy = false;
                    }
                    _setLockScreenCommand.RaiseCanExecuteChanged();
                }
            }, () =>
            {
                return !LockScreenHelper.HasAccess();
            });

            _backupCommand = new DelegateCommand(() =>
            {
#if !DEBUG
                if (!InAppPurchaseHelper.IsProductActive(AppConstants.IAP_AWESOME_EDITION))
                {
                    NavigationService.Navigate(new Uri("/Pages/InAppStorePage.xaml", UriKind.Relative));
                }
                else
#endif
                {
                    NavigationService.Navigate(new Uri("/Pages/BackupPage.xaml", UriKind.Relative));
                }
            }, () =>
            {
                return true;
            });
        }

        public async Task UpdateLockScreenAsync()
        {
            var data = await _bashClient.GetQuotesAsync(AppConstants.ORDER_VALUE_RANDOM, AppConstants.QUOTES_COUNT, 0);
            BashLockscreenHelper.UpdateAsync(data);
            return;
        }

        public void UpdateBackgroundTask()
        {
            if (!LockScreenHelper.HasAccess())
                return;

            var task = new PeriodicTask(AppConstants.BACKGROUND_TASK_NAME)
            {
                Description = AppConstants.BACKGROUND_TASK_DESC
            };

            BackgroundTaskHelper.StartTask(task);
        }

        #endregion

        #region Properties

        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                if (value != _searchTerm)
                {
                    _searchTerm = value;
                    NotifyPropertyChanged("SearchTerm");
                    _searchCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool CanSearch
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_searchTerm);
            }
        }

        public NavigationService NavigationService
        {
            set { _navigationService = value; }
            private get { return _navigationService; }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    NotifyPropertyChanged("IsBusy");
                }
            }
        }

        public ICommand SearchCommand
        {
            get { return _searchCommand; }
        }

        public ICommand SetLockScreenCommand
        {
            get { return _setLockScreenCommand; }
        }

        public ICommand BackupCommand
        {
            get { return _backupCommand; }
        }

        #endregion
    }
}
