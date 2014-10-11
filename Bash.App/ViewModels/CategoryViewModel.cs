﻿using Bash.App.Data;
using Bash.App.Models;
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
        private DelegateCommand _shareWhatsAppCommand;
        private DelegateCommand _shareClipboardCommand;
        private DelegateCommand _shareLinkCommand;
        private DelegateCommand _shareContentCommand;
        private DelegateCommand _jumpToCommand;
        private DelegateCommand _refreshCommand;
        private DelegateCommand _openInBrowserCommand;

        private bool _isBusy;

        private string _jumpPageNumber;

        private readonly StoredObject<List<int>> _storedRatings = new StoredObject<List<int>>("__storedRating", new List<int>());

        public static bool WasLastNavigationNext { get; set; }

        public CategoryState CategoryState { get; set; }

        public string LastSearchTerm { get; private set; }

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
            IsBusy = true;
            var result = await _bashClient.GetQuotesAsync(order, AppConstants.QUOTES_COUNT, 0, forceReload);

            if (result == null)
            {
                IsBusy = false;
                return false;
            }


            if (order == AppConstants.ORDER_VALUE_RANDOM)
            {
                result.Contents.Data.ShuffleList(); // TODO make sure after a tombstone, the same item is selected?
            }

            BashCollection = result;
            IsBusy = false;
            
            // set category state
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

            return true;
        }

        public bool LoadFavorites()
        {
            BashCollection = _favoriteManager.GetData();
            CategoryState = CategoryState.Favorites;
            return true;
        }

        public async Task<bool> SearchQuotesAsync(string term)
        {
            LastSearchTerm = term;
            var result = await _bashClient.GetQueryAsync(term, AppConstants.QUOTES_COUNT, 0);
            CategoryState = CategoryState.Search;

            if (result == null)
                return false;

            BashCollection = result;
            return true;
        }
        
        public void Reset()
        {
            CurrentBashDataIndex = 0;
            BashCollection = null;
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            _nextCommand = new DelegateCommand(() =>
            {
                WasLastNavigationNext = true;
                CurrentBashDataIndex++;
            },
            () =>
            {
                return CanNext;
            });

            _previousCommand = new DelegateCommand(() =>
            {
                WasLastNavigationNext = false;
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

        private bool IsBashUnrated(BashData bashData)
        {
            return bashData != null && !_storedRatings.Value.Contains(bashData.Id);
        }

        #endregion

        #region Properties

        public int CurrentBashDataIndex
        {
            private get { return _currentBashDataIndex; }
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
                UpdateRatingCommands();
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
            }
        }

        public BashData CurrentBashData
        {
            get
            {
                if (BashCollection == null || BashCollection.Contents.Data.Count <= CurrentBashDataIndex)
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
