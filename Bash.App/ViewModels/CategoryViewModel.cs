using Bash.App.Data;
using Bash.App.Models;
using PhoneKit.Framework.Core.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Bash.App.ViewModels
{
    public class CategoryViewModel : ViewModelBase, ICategoryViewModel
    {
        #region Members

        NavigationService _navigationService;

        private IBashClient _bashClient;

        private BashCollection _bashCollection;
        private int _currentBashDataIndex;

        private DelegateCommand _nextCommand;
        private DelegateCommand _previousCommand;
        private DelegateCommand _ratePositiveCommand;
        private DelegateCommand _rateNegativeCommand;
        private DelegateCommand _showCommentsCommand;

        #endregion

        #region Constructors

        public CategoryViewModel(ICachedBashClient bashClient)
        {
            InitializeCommands();
            _bashClient = bashClient;
        }

        #endregion

        #region Public Methods

        public async Task<bool> LoadQuotesAsync(string order)
        {
            var result = await _bashClient.GetQuotesAsync(order, AppConstants.QUOTES_COUNT, 0);

            if (result == null)
                return false;

            BashCollection = result;
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
        }

        #endregion

        #region Properties

        private int CurrentBashDataIndex
        {
            get { return _currentBashDataIndex; }
            set
            {
                _currentBashDataIndex = value;
                NotifyPropertyChanged("CurrentBashData");
                _nextCommand.RaiseCanExecuteChanged();
                _previousCommand.RaiseCanExecuteChanged();
                _showCommentsCommand.RaiseCanExecuteChanged();
            }
        }

        private BashCollection BashCollection {
            get { return _bashCollection; }
            set
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
                if (BashCollection == null)
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

        #endregion
    }
}
