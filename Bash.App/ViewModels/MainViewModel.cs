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
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        #region Members

        private string _searchTerm;

        private NavigationService _navigationService;

        private DelegateCommand _searchCommand;

        #endregion

        #region Constructors

        public MainViewModel()
        {
            InitializeCommands();
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
                    _navigationService.Navigate(new Uri(uriString, UriKind.Relative));
                }
                
            }, () =>
            {
                return true;
            });
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

        #endregion
    }
}
