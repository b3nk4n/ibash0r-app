using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Bash.App.ViewModels;
using Ninject;
using Bash.App.Data;
using System;
using Microsoft.Phone.Shell;
using Bash.App.Resources;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Microsoft.Phone.Tasks;

namespace Bash.App.Pages
{
    public partial class CategoryPage : PhoneApplicationPage
    {
        private const int SWIPE_LIMIT = 1250;

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
                    if (_categoryViewModel.NextCommand.CanExecute(null))
                    {
                        _categoryViewModel.NextCommand.Execute(null);
                        ResetScroller();
                    }
                        
                }
                else if (velocity.X > SWIPE_LIMIT)
                {
                    if (_categoryViewModel.PreviousCommand.CanExecute(null))
                    {
                        _categoryViewModel.PreviousCommand.Execute(null);
                        ResetScroller();
                    }
                }
            };
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // setup view model
            _categoryViewModel.NavigationService = NavigationService;
            _categoryViewModel.Reset();
            DataContext = _categoryViewModel;

            bool success = false;
            if (NavigationContext.QueryString.ContainsKey(AppConstants.PARAM_ORDER))
            {
                var order = NavigationContext.QueryString[AppConstants.PARAM_ORDER];

                switch (order)
                {
                    case AppConstants.ORDER_VALUE_BEST:
                        PageTitle.Text = AppResources.CategoryBestQuotes;
                        break;
                    case AppConstants.ORDER_VALUE_NEW:
                        PageTitle.Text = AppResources.CategoryNewQuotes;
                        break;
                    case AppConstants.ORDER_VALUE_RANDOM:
                        PageTitle.Text = AppResources.CategoryRandomQuotes;
                        break;
                }

                success = await _categoryViewModel.LoadQuotesAsync(order);
            }
            else if (NavigationContext.QueryString.ContainsKey(AppConstants.PARAM_TERM))
            {
                PageTitle.Text = AppResources.CategorySearchQuotes;
                var term = NavigationContext.QueryString[AppConstants.PARAM_TERM];
                success = await _categoryViewModel.SearchQuotesAsync(term);
            }
            else if (NavigationContext.QueryString.ContainsKey(AppConstants.PARAM_FAVORITES))
            {
                PageTitle.Text = AppResources.CategoryFavoriteQuotes;
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

        private void QuoteItemLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var grid = sender as Grid;

            if (grid != null)
            {
                Storyboard storyboard;
                if (CategoryViewModel.WasLastNavigationNext)
                    storyboard = grid.Resources["FadeInRightToLeft"] as Storyboard;
                else
                    storyboard = grid.Resources["FadeInLeftToRight"] as Storyboard;
                var animationIndex = (int)grid.Tag;
                storyboard.BeginTime = (TimeSpan.FromMilliseconds(5 + 33 * animationIndex));
                storyboard.Begin();
            }
        }

        private void ScrollUpHander(object sender, EventArgs e)
        {
            ResetScroller();
        }

        private void ResetScroller()
        {
            QuotesScroller.ScrollToVerticalOffset(0.0);
        }

        private void ShareClicked(object sender, EventArgs e)
        {
            //var shareTask = new ShareLinkTask();
            //shareTask.LinkUri = new Uri("http://www.google.de");
            //shareTask.Message = "test";
            //shareTask.Title = "Share title";
            //shareTask.Show();

            //var shareTask = new ShareStatusTask();
            //shareTask.Status = "Das ist mein Status";
            //shareTask.Show();
            ShowShareBar.Begin();
        }

        private void ContentPanelTapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideShareBar.Begin();
        }
    }
}