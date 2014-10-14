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
using PhoneKit.Framework.Tile;
using PhoneKit.Framework.Core.Tile;
using Microsoft.Phone.Shell;
using Bash.App.Resources;

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
        private DelegateCommand<string> _pinToStartCommand;
        private DelegateCommand<string> _pinToStartProCommand;

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

        public void Update()
        {
            NotifyPropertyChanged("IsAwesomeEdition");
        }

        public async Task UpdateLockScreenAsync()
        {
            var data = await _bashClient.GetQuotesAsync(AppConstants.ORDER_VALUE_RANDOM, AppConstants.QUOTES_COUNT_PRELOAD, 0);
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
                if (!LicenceEasterEggHelper.IsAwesomeEditionUnlocked())
                {
                    NavigationService.Navigate(new Uri("/Pages/InAppStorePage.xaml", UriKind.Relative));
                }
                else
                {
                    if (!LockScreenHelper.HasAccess())
                    {
                        if (await LockScreenHelper.VerifyAccessAsync())
                        {
                            await UpdateLockScreenAsync();
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
                if (!LicenceEasterEggHelper.IsAwesomeEditionUnlocked())
                {
                    NavigationService.Navigate(new Uri("/Pages/InAppStorePage.xaml", UriKind.Relative));
                }
                else
                {
                    NavigationService.Navigate(new Uri("/Pages/BackupPage.xaml", UriKind.Relative));
                }
            }, () =>
            {
                return true;
            });

            _pinToStartCommand = new DelegateCommand<string>((param) =>
            {
                string[] splitted = param.Split(';');
                string category = splitted[0];
                string navigationString = splitted[1];

                PinTileToStart(category, navigationString);
            }, (param) =>
            {
                string[] splitted = param.Split(';');
                string navigationString = splitted[1];

                return !LiveTileHelper.TileExists(new Uri(navigationString, UriKind.Relative));
            });

            _pinToStartProCommand = new DelegateCommand<string>((param) =>
            {
                string[] splitted = param.Split(';');
                string category = splitted[0];
                string navigationString = splitted[1];

                PinTileToStart(category, navigationString);
            }, (param) =>
            {
                if (!LicenceEasterEggHelper.IsAwesomeEditionUnlocked())
                    return false;

                string[] splitted = param.Split(';');
                string navigationString = splitted[1];

                return !LiveTileHelper.TileExists(new Uri(navigationString, UriKind.Relative));
            });
        }

        private void PinTileToStart(string category, string navigationString)
        {
            var tile = new StandardTileData
            {
                Title = AppResources.ApplicationTitle,
                BackgroundImage = new Uri(string.Format("/Assets/{0}.png", category), UriKind.Relative),
                BackTitle = MapToLocalizedTitleResource(category),
                BackBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileMedium.png", UriKind.Relative)
            };

            LiveTilePinningHelper.PinOrUpdateTile(new Uri(navigationString, UriKind.Relative), tile);
        }

        private string MapToLocalizedTitleResource(string splitted)
        {
            switch (splitted)
            {
                case AppConstants.ORDER_VALUE_BEST:
                    return AppResources.CategoryBestQuotes;
                case AppConstants.ORDER_VALUE_NEW:
                    return AppResources.CategoryNewQuotes;
                case AppConstants.ORDER_VALUE_RANDOM:
                    return AppResources.CategoryRandomQuotes;
                case AppConstants.PARAM_FAVORITES:
                    return AppResources.CategoryFavoriteQuotes;
            }
            return string.Empty;
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

        public bool IsAwesomeEdition
        {
            get
            {
                return LicenceEasterEggHelper.IsAwesomeEditionUnlocked();
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

        public ICommand PinToStartCommand
        {
            get { return _pinToStartCommand; }
        }

        public ICommand PinToStartProCommand
        {
            get { return _pinToStartProCommand; }
        }

        #endregion
    }
}
