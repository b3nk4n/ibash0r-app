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
        public const string PARAM_FAVORITES = "favorites";

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

            if (NavigationContext.QueryString.ContainsKey(BashClient.PARAM_ORDER))
            {
                var order = NavigationContext.QueryString[BashClient.PARAM_ORDER];
                await _categoryViewModel.LoadQuotesAsync(order);
            }
            else if (NavigationContext.QueryString.ContainsKey(BashClient.PARAM_TERM))
            {
                var term = NavigationContext.QueryString[BashClient.PARAM_TERM];
                await _categoryViewModel.SearchQuotesAsync(term);
            }
            else if (NavigationContext.QueryString.ContainsKey(PARAM_FAVORITES))
            {
                // load data from favorites list.
            }
            else
            {
                throw new ArgumentException("Paremeters required.");
            }
        }
    }
}