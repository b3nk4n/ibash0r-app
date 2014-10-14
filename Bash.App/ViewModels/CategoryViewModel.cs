using Bash.Common.Data;
using Bash.Common.Models;
using PhoneKit.Framework.Core.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using PhoneKit.Framework.Core.Collections;
using System.ComponentModel;
using PhoneKit.Framework.Core.Storage;
using System.Windows;
using Bash.App.Resources;
using Microsoft.Phone.Tasks;
using Bash.Common;
using PhoneKit.Framework.Storage;

namespace Bash.App.ViewModels
{
    public class CategoryViewModel : ViewModelBase, ICategoryViewModel
    {
        #region Members

        NavigationService _navigationService;

        private ICachedBashClient _bashClient;
        private IFavoriteManager _favoriteManager;

        private BashCollection _bashCollection;
        private int _currentBashDataIndex;

        private DelegateCommand _nextCommand;
        private DelegateCommand _previousCommand;
        private DelegateCommand _ratePositiveCommand;
        private DelegateCommand _rateNegativeCommand;
        private DelegateCommand _showCommentsCommand;
        private DelegateCommand _addToFavoritesCommand;
        private DelegateCommand _removeFromFavoritesCommand;
        private DelegateCommand _placeholderCommand;
        private DelegateCommand _shareWhatsAppCommand;
        private DelegateCommand _shareClipboardCommand;
        private DelegateCommand _shareLinkCommand;
        private DelegateCommand _shareContentCommand;
        private DelegateCommand _jumpToCommand;
        private DelegateCommand _refreshCommand;
        private DelegateCommand _openInBrowserCommand;

        private const string KEY_CURRENT_INDEX = "current_index";
        private const string KEY_CURRENT_ID = "current_id";

        private bool _isBusy;

        private string _jumpPageNumber;

        private readonly StoredObject<List<int>> _storedRatings = new StoredObject<List<int>>("__storedRating", new List<int>());

        public static bool WasLastNavigationNext { get; set; }

        public CategoryState CategoryState { get; set; }

        /// <summary>
        /// Freshly loaded data to indicate, that the data is new and shown the first time.
        /// Used for the quotes animations.
        /// </summary>
        private bool? _isDataFreshlyLoaded = null;

        private bool _showLoadingFaildInfo;

        #endregion

        #region Constructors

        public CategoryViewModel() {
            InitializeCommands();
            _bashClient = new CachedBashClient(null);
            _favoriteManager = new FavoriteManager();
        } // for sample data

        public CategoryViewModel(ICachedBashClient bashClient, IFavoriteManager favoriteManager)
        {
            InitializeCommands();
            _bashClient = bashClient;
            _favoriteManager = favoriteManager;
        }

        #endregion

        #region Public Methods

        public async Task<bool> LoadQuotesAsync(string order, bool forceReload = false)
        {
            // set category state
            UpdateCategoryState(order);

            IsDataFreshlyLoaded = true;

            IsBusy = true;
            var result = await _bashClient.GetQuotesAsync(order, AppConstants.QUOTES_COUNT, 0, CachedBashClient.LIFE_TIME_DAYS_DEFAULT, forceReload);

            if (result == null)
            {
                IsBusy = false;
                ShowLoadingFailedInfo = true;
                return false;
            }

            if (order == AppConstants.ORDER_VALUE_RANDOM)
            {
                result.Contents.Data.ShuffleList();
            }

            BashCollection = result;
            IsBusy = false;
            return true;
        }

        public async Task<bool> LoadWarteAsync(bool forceReload = false)
        {
            CategoryState = CategoryState.Warte;
            IsDataFreshlyLoaded = true;

            IsBusy = true;
            var result = await _bashClient.GetWarteAsync(forceReload);

            if (result == null)
            {
                IsBusy = false;
                ShowLoadingFailedInfo = true;
                return false;
            }

            BashCollection = result;
            IsBusy = false;
            return true;
        }

        private void UpdateCategoryState(string order)
        {
            switch (order)
            {
                case AppConstants.ORDER_VALUE_BEST:
                    CategoryState = CategoryState.Best;
                    break;
                case AppConstants.ORDER_VALUE_NEW:
                    CategoryState = CategoryState.New;
                    break;
                case AppConstants.ORDER_VALUE_RANDOM:
                    CategoryState = CategoryState.Random;
                    break;
            }
        }

        public bool LoadFavorites()
        {
            IsDataFreshlyLoaded = true;
            CategoryState = CategoryState.Favorites;
            BashCollection = _favoriteManager.GetData();
            return true;
        }

        public async Task<bool> SearchQuotesAsync(string term)
        {
            CategoryState = CategoryState.Search;
            IsDataFreshlyLoaded = true;
            IsBusy = true;
            var result = await _bashClient.GetQueryAsync(term, AppConstants.QUOTES_COUNT, 0);

            if (result == null)
            {
                ShowLoadingFailedInfo = true;
                IsBusy = false;
                return false;
            }

            BashCollection = result;
            IsBusy = false;
            return true;
        }
        
        public void Reset()
        {
            ShowLoadingFailedInfo = false;
            CurrentBashDataIndex = 0;
            BashCollection = null;
        }

       public void SaveState()
        {
            PhoneStateHelper.SaveValue(KEY_CURRENT_INDEX, CurrentBashDataIndex);

            if (CurrentBashData != null && CategoryState == ViewModels.CategoryState.Random)
                PhoneStateHelper.SaveValue(KEY_CURRENT_ID, CurrentBashData.Id); // to restore the current item for random category
        }

        public void RestoreState()
        {
            int index = -1;
            if (PhoneStateHelper.ValueExists(KEY_CURRENT_INDEX))
            {
                index = PhoneStateHelper.LoadValue<int>(KEY_CURRENT_INDEX);
                CurrentBashDataIndex = Math.Min(index, BashCount - 1);
            }
            if (index != -1 && CurrentBashData != null && BashCollection != null && BashCount > 1)
            {
                if (PhoneStateHelper.ValueExists(KEY_CURRENT_ID))
                {
                    var id = PhoneStateHelper.LoadValue<int>(KEY_CURRENT_ID);
                    int counter = 0;
                    int? position = null;
                    
                    // move the bash data with the id to the current index
                    foreach (var item in BashCollection.Contents.Data)
                    {
                        if (item.Id == id)
                        {
                            position = counter;
                            break;
                        }
                        ++counter;
                    }

                    if (position != null)
                    {
                        // swap
                        var tmp = BashCollection.Contents.Data[position.Value];
                        BashCollection.Contents.Data[position.Value] = BashCollection.Contents.Data[index];
                        BashCollection.Contents.Data[index] = tmp;
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            _nextCommand = new DelegateCommand(() =>
            {
                WasLastNavigationNext = true;
                InvalidateIsDataFreshlyLoaded();
                CurrentBashDataIndex++;
            },
            () =>
            {
                return CanNext;
            });

            _previousCommand = new DelegateCommand(() =>
            {
                WasLastNavigationNext = false;
                InvalidateIsDataFreshlyLoaded();
                CurrentBashDataIndex--;
            },
            () =>
            {
                return CanPrevious;
            });

            _ratePositiveCommand = new DelegateCommand(async () =>
            {
                IsBusy = true;
                if (await _bashClient.RateAsync(CurrentBashData.Id, AppConstants.TYPE_VALUE_POS))
                {
                    _storedRatings.Value.Add(CurrentBashData.Id);
                    CurrentBashData.Rating++;
                    _bashClient.UpdateCache(BashCollection);
                }
                UpdateRatingCommands();
                IsBusy = false;
            },
            () =>
            {
                return IsBashUnrated(CurrentBashData);
            });

            _rateNegativeCommand = new DelegateCommand(async () =>
            {
                IsBusy = true;
                if (await _bashClient.RateAsync(CurrentBashData.Id, AppConstants.TYPE_VALUE_NEG))
                {
                    _storedRatings.Value.Add(CurrentBashData.Id);
                    CurrentBashData.Rating--;
                    _bashClient.UpdateCache(BashCollection);
                }
                UpdateRatingCommands();
                IsBusy = false;
            },
            () =>
            {
                return IsBashUnrated(CurrentBashData);
            });

            _showCommentsCommand = new DelegateCommand(() =>
            {
                var uriString = string.Format("/Pages/CommentsPage.xaml?{0}={1}", AppConstants.PARAM_ID, CurrentBashData.Id);
                NavigationService.Navigate(new Uri(uriString, UriKind.Relative));
            },
            () =>
            {
                return CurrentBashData != null;
            });

            _addToFavoritesCommand = new DelegateCommand(() =>
            {
                _favoriteManager.AddToFavorites(CurrentBashData);
                NotifyPropertyChanged("IsCurrentBashFavorite");
            },
            () =>
            {
                return CurrentBashData != null;
            });

            _removeFromFavoritesCommand = new DelegateCommand(() =>
            {
                _favoriteManager.RemoveFromFavorites(CurrentBashData);
                NotifyPropertyChanged("IsCurrentBashFavorite");
            },
            () =>
            {
                return CurrentBashData != null;
            });

            _placeholderCommand = new DelegateCommand(() =>
            {
                // NOP
            },
            () =>
            {
                return CurrentBashData != null;
            });

            _shareContentCommand = new DelegateCommand(() =>
            {
                var task = new ShareStatusTask();
                task.Status = CurrentBashData.QuoteString;
                task.Show();
            },
            () =>
            {
                return CurrentBashData != null;
            });

            _shareLinkCommand = new DelegateCommand(() =>
            {
                var task = new ShareLinkTask();
                task.Title = AppResources.ShareLinkTitle;
                task.LinkUri = CurrentBashData.Uri;
                task.Show();
            },
            () =>
            {
                return CurrentBashData != null;
            });

            _shareClipboardCommand = new DelegateCommand(() =>
            {
                Clipboard.SetText(CurrentBashData.QuoteString);
                MessageBox.Show(AppResources.MessageBoxInfoClipboard, AppResources.MessageBoxInfoTitle, MessageBoxButton.OK);
            },
            () =>
            {
                return CurrentBashData != null;
            });

            _shareWhatsAppCommand = new DelegateCommand(async () =>
            {
                if (MessageBox.Show(AppResources.MessageBoxInfoWhatsapp, AppResources.MessageBoxInfoTitle, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Clipboard.SetText(CurrentBashData.QuoteString);
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("whatsapp:"));
                }
            },
            () =>
            {
                return CurrentBashData != null;
            });

            _jumpToCommand = new DelegateCommand(() =>
            {
                int number = 0;
                if (int.TryParse(JumpPageNumber, out number))
                {
                    CurrentBashDataIndex = (int)Math.Min((int)Math.Max(number, 1), BashCount) - 1;
                }
            },
            () =>
            {
                return CurrentBashData != null;
            });

            _refreshCommand = new DelegateCommand(async () =>
            {
                BashCollection = null;

                switch (CategoryState)
	            {
                    case CategoryState.New:
                        await LoadQuotesAsync(AppConstants.ORDER_VALUE_NEW, true);
                        break;
                    case CategoryState.Best:
                        await LoadQuotesAsync(AppConstants.ORDER_VALUE_BEST, true);
                        break;
                    case CategoryState.Search:
                        // NOP, because the result might be the same.
                        break;
                    case CategoryState.Random:
                        await LoadQuotesAsync(AppConstants.ORDER_VALUE_RANDOM, true);
                        break;
                    case CategoryState.Favorites:
                        _favoriteManager.SaveData();
                        LoadFavorites();
                        break;
	            }
            },
            () =>
            {
                return CurrentBashData != null && CategoryState != ViewModels.CategoryState.Search;
            });

            _openInBrowserCommand = new DelegateCommand(() =>
            {
                var browserTask = new WebBrowserTask();
                browserTask.Uri = CurrentBashData.Uri;
                browserTask.Show();
            },
            () =>
            {
                return CurrentBashData != null;
            });
        }

        private void UpdateRatingCommands()
        {
            _rateNegativeCommand.RaiseCanExecuteChanged();
            _ratePositiveCommand.RaiseCanExecuteChanged();
        }

        private void UpdateShareCommands()
        {
            _shareClipboardCommand.RaiseCanExecuteChanged();
            _shareContentCommand.RaiseCanExecuteChanged();
            _shareLinkCommand.RaiseCanExecuteChanged();
            _shareWhatsAppCommand.RaiseCanExecuteChanged();
        }

        private bool IsBashUnrated(BashData bashData)
        {
            return bashData != null && !_storedRatings.Value.Contains(bashData.Id);
        }

        private void InvalidateIsDataFreshlyLoaded()
        {
            if (IsDataFreshlyLoaded)
            {
                IsDataFreshlyLoaded = false;
            }
        }

        #endregion

        #region Properties

        public int CurrentBashDataIndex
        {
            get { return _currentBashDataIndex; }
            set // sample data only
            {
                _currentBashDataIndex = value;
                NotifyPropertyChanged("CurrentBashData");
                NotifyPropertyChanged("IsCurrentBashFavorite");
                NotifyPropertyChanged("BashNumber");
                _nextCommand.RaiseCanExecuteChanged();
                _previousCommand.RaiseCanExecuteChanged();
                _showCommentsCommand.RaiseCanExecuteChanged();
                _addToFavoritesCommand.RaiseCanExecuteChanged();
                _refreshCommand.RaiseCanExecuteChanged();
                _openInBrowserCommand.RaiseCanExecuteChanged();
                _jumpToCommand.RaiseCanExecuteChanged();
                _placeholderCommand.RaiseCanExecuteChanged();
                UpdateRatingCommands();
                UpdateShareCommands();
            }
        }

        public BashCollection BashCollection {
            private get { return _bashCollection; }
            set // sample data only
            {
                _bashCollection = value;
                
                // select the first one
                CurrentBashDataIndex = 0;
                NotifyPropertyChanged("BashCount");
                NotifyPropertyChanged("ShowFavoritesInfo");
            }
        }

        public BashData CurrentBashData
        {
            get
            {
                if (BashCollection == null || BashCollection.Contents.Data.Count <= CurrentBashDataIndex || CurrentBashDataIndex < 0)
                    return null;
                return BashCollection.Contents.Data[CurrentBashDataIndex];
            }
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
                    NotifyPropertyChanged("ShowSearchNoResultsInfo");
                }
            }
        }

        public int BashNumber
        {
            get
            {
                return _currentBashDataIndex + 1;
            }
        }

        public int BashCount
        {
            get
            {
                return BashCollection == null ? 0 : BashCollection.Contents.Data.Count;
            }
        }

        public string JumpPageNumber
        {
            get { return _jumpPageNumber; }
            set
            {
                if (value != _jumpPageNumber)
                {
                    _jumpPageNumber = value;
                    NotifyPropertyChanged("JumpPageNumber");
                    _jumpToCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool ShowFavoritesInfo
        {
            get
            {
                if (_favoriteManager == null || CategoryState != ViewModels.CategoryState.Favorites)
                    return false;
                return _favoriteManager.GetData().Contents.Data.Count == 0;
            }
        }

        public bool ShowSearchNoResultsInfo
        {
            get
            {
                if (CategoryState != ViewModels.CategoryState.Search)
                    return false;
                return BashCount == 0 && !IsBusy;
            }
        }

        public bool ShowLoadingFailedInfo
        {
            private set
            {
                if (_showLoadingFaildInfo != value)
                {
                    _showLoadingFaildInfo = value;
                    NotifyPropertyChanged("ShowLoadingFailedInfo");
                }
            }
            get
            {
                return _showLoadingFaildInfo; ;
            }
        }

        public bool IsDataFreshlyLoaded
        {
            get
            {
                if (_isDataFreshlyLoaded == null)
                    return true;
                return _isDataFreshlyLoaded.Value;
            }
            set
            {
                _isDataFreshlyLoaded = value;
            }
        }

        private bool CanPrevious
        {
            get { return BashCollection != null && CurrentBashDataIndex > 0; }
        }

        private bool CanNext
        {
            get { return BashCollection != null && CurrentBashDataIndex < BashCollection.Contents.Data.Count - 1; }
        }

        public NavigationService NavigationService
        {
            set { _navigationService = value; }
            private get { return _navigationService; }
        }

        public bool IsCurrentBashFavorite
        {
            get { return _favoriteManager.IsFavorite(CurrentBashData); }
        }

        public ICommand NextCommand
        {
            get { return _nextCommand; }
        }

        public ICommand PreviousCommand
        {
            get { return _previousCommand; }
        }

        public ICommand RatePositiveCommand
        {
            get { return _ratePositiveCommand; }
        }

        public ICommand RateNegativeCommand
        {
            get { return _rateNegativeCommand; }
        }

        public ICommand ShowCommentsCommand
        {
            get { return _showCommentsCommand; }
        }

        public ICommand AddToFavoritesCommand
        {
            get { return _addToFavoritesCommand; }
        }

        public ICommand RemoveFromFavoritesCommand
        {
            get { return _removeFromFavoritesCommand; }
        }

        public ICommand PlaceholderCommand
        {
            get { return _placeholderCommand; }
        }

        public ICommand ShareWhatsAppCommand
        {
            get { return _shareWhatsAppCommand; }
        }

        public ICommand ShareClipboardCommand
        {
            get { return _shareClipboardCommand; }
        }

        public ICommand ShareLinkCommand
        {
            get { return _shareLinkCommand; }
        }

        public ICommand ShareContentCommand
        {
            get { return _shareContentCommand; }
        }

        public ICommand JumpToCommand
        {
            get { return _jumpToCommand; }
        }

        public ICommand RefreshCommand
        {
            get { return _refreshCommand; }
        }

        public ICommand OpenInBrowserCommand
        {
            get { return _openInBrowserCommand; }
        }

        #endregion
    }
}
