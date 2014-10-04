using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Bash.App.ViewModels;
using Ninject;
using Bash.App.Data;
using System;
using Microsoft.Phone.Shell;

namespace Bash.App.Pages
{
    public partial class CategoryPage : PhoneApplicationPage
    {
        private ICategoryViewModel _categoryViewModel;

        public CategoryPage()
        {
            InitializeComponent();
            _categoryViewModel = App.Injector.Get<ICategoryViewModel>();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // setup view model
            _categoryViewModel.NavigationService = NavigationService;
            DataContext = _categoryViewModel;

            bool success = false;
            if (NavigationContext.QueryString.ContainsKey(AppConstants.PARAM_ORDER))
            {
                var order = NavigationContext.QueryString[AppConstants.PARAM_ORDER];
                success = await _categoryViewModel.LoadQuotesAsync(order);
            }
            else if (NavigationContext.QueryString.ContainsKey(AppConstants.PARAM_TERM))
            {
                var term = NavigationContext.QueryString[AppConstants.PARAM_TERM];
                success = await _categoryViewModel.SearchQuotesAsync(term);
            }
            else if (NavigationContext.QueryString.ContainsKey(AppConstants.PARAM_FAVORITES))
            {
                // load data from favorites list.
                success = _categoryViewModel.LoadFavorites();
            }
            else
            {
                throw new ArgumentException("Paremeters required.");
            }

            if (!success)
            {
                // TODO: handle error?
            }
        }
    }
}