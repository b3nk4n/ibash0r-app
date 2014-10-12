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
                            UpdateLockScreen();
                        }
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

        /// <summary>
        /// The persistent name of the next lockscreen image to toggle from A to B,
        /// which is required for lockscreen image update.
        /// </summary>
        private StoredObject<string> _nextLockScreenExtension = new StoredObject<string>("__nextLockScreenExtension", "A");

        private Random random = new Random();

        private async void UpdateLockScreen()
        {
            if (!LockScreenHelper.HasAccess())
                return;

            WriteableBitmap lockGfx;
            Uri lockUri;

            // get data
            var data = await _bashClient.GetQuotesAsync(AppConstants.ORDER_VALUE_RANDOM, AppConstants.QUOTES_COUNT, 0);
            if (data == null || data.Contents.Data.Count == 0)
                return;

            // find quote
            int index = -1;
            for (int retry = 0; retry < 15; retry++)
            {
                int i = random.Next(data.Contents.Data.Count);

                if (data.Contents.Data[i].GuessHeightScore() < 14)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                return;

            // render image
            lockGfx = GraphicsHelper.Create(new LockQuoteControl(data.Contents.Data[index]));

            // save lock image
            var nextExtension = _nextLockScreenExtension.Value;
            lockUri = StorageHelper.SaveJpeg(string.Format("/lockquote_{0}.jpg", nextExtension), lockGfx);
            _nextLockScreenExtension.Value = (nextExtension == "A") ? "B" : "A";

            // set lockscreen image
            LockScreenHelper.SetLockScreenImage(lockUri, true);
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
