using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Bash.App.ViewModels;
using Ninject;

namespace Bash.App.Pages
{
    public partial class SearchPage : PhoneApplicationPage
    {
        private ISearchViewModel _searchViewModel;

        public SearchPage()
        {
            InitializeComponent();
            _searchViewModel = App.Injector.Get<ISearchViewModel>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // setup view model
            _searchViewModel.NavigationService = NavigationService;
            DataContext = _searchViewModel;
        }
    }
}