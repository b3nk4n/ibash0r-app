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
        private const int SWIPE_LIMIT = 1750;

        private ICategoryViewModel _categoryViewModel;

        public CategoryPage()
        {
            InitializeComponent();
            _categoryViewModel = App.Injector.Get<ICategoryViewModel>();

            QuoteList.ManipulationCompleted += (s, e) =>
            {
                var velocity = e.FinalVelocities.LinearVelocity;

                if (velocity.Y != 0)
                    return;

                if (velocity.X < -SWIPE_LIMIT)
                {
                    if (_categoryViewModel.PreviousCommand.CanExecute(null))
                        _categoryViewModel.PreviousCommand.Execute(null);
                }
                else if (velocity.X > SWIPE_LIMIT)
                {
                    if (_categoryViewModel.NextCommand.CanExecute(null))
                        _categoryViewModel.NextCommand.Execute(null);
                }
            };
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