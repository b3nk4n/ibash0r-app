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

namespace Bash.App.ViewModels
{
    public class CategoryViewModel : ViewModelBase, ICategoryViewModel
    {
        #region Members

        NavigationService _navigationService;

        private IBashClient _bashClient;
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

        #endregion

        #region Constructors

        public CategoryViewModel() {
            InitializeCommands();
            _bashClient = new BashClient();
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

        public async Task<bool> LoadQuotesAsync(string order)
        {
            var result = await _bashClient.GetQuotesAsync(order, AppConstants.QUOTES_COUNT, 0);

            if (result == null)
                return false;

            if (order == AppConstants.ORDER_VALUE_RANDOM)
            {
                result.Contents.Data.ShuffleList(); // TODO make sure after a tombstone, the same item is selected?
            }

            BashCollection = result;
            return true;
        }

        public bool LoadFavorites()
        {
            BashCollection = _favoriteManager.GetData();
            return true;
        }

        public async Task<bool> SearchQuotesAsync(string term)
        {
            var result = await _bashClient.GetQueryAsync(term, AppConstants.QUOTES_COUNT, 0);

            if (result == null)
                return false;

            BashCollection = result;
            return true;
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            _nextCommand = new DelegateCommand(() =>
            {
                CurrentBashDataIndex++;
            },
            () =>
            {
                return CanNext;
            });

            _previousCommand = new DelegateCommand(() =>
            {
                CurrentBashDataIndex--;
            },
            () =>
            {
                return CanPrevious;
            });

            _ratePositiveCommand = new DelegateCommand(async () =>
            {
                await _bashClient.RateAsync(CurrentBashData.Id, AppConstants.TYPE_VALUE_POS);
            },
            () =>
            {
                return CurrentBashData != null;
            });

            _rateNegativeCommand = new DelegateCommand(async () =>
            {
                await _bashClient.RateAsync(CurrentBashData.Id, AppConstants.TYPE_VALUE_NEG);
            },
            () =>
            {
                return CurrentBashData != null;
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
                _nextCommand.RaiseCanExecuteChanged();
                _previousCommand.RaiseCanExecuteChanged();
                _showCommentsCommand.RaiseCanExecuteChanged();
                _addToFavoritesCommand.RaiseCanExecuteChanged();
            }
        }

        public BashCollection BashCollection {
            private get { return _bashCollection; }
            set // sample data only
            {
                _bashCollection = value;
                
                // select the first one
                CurrentBashDataIndex = 0;
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

        #endregion
    }
}
