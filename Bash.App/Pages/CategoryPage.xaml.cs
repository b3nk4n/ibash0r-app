using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Bash.App.ViewModels;
using Ninject;
using Bash.Common.Data;
using System;
using Microsoft.Phone.Shell;
using Bash.App.Resources;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Microsoft.Phone.Tasks;
using Bash.Common;
using PhoneKit.Framework.Storage;

namespace Bash.App.Pages
{
    public partial class CategoryPage : PhoneApplicationPage
    {
        private const int SWIPE_LIMIT = 1000;

        private ICategoryViewModel _categoryViewModel;
        private IFavoriteManager _favoriteManager;

        public CategoryPage()
        {
            InitializeComponent();
            _categoryViewModel = App.Injector.Get<ICategoryViewModel>();
            _favoriteManager = App.Injector.Get<IFavoriteManager>();

            QuotesScroller.ManipulationCompleted += (s, e) =>
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

            HideJumpBar.Completed += (s, e) =>
                {
                    phoneTextBox.Text = string.Empty;
                };
        }

        private const string KEY_CURRENT_INDEX = "current_index";

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

            if (!e.IsNavigationInitiator)
            {
                if (PhoneStateHelper.ValueExists(KEY_CURRENT_INDEX))
                {
                      var index = PhoneStateHelper.LoadValue<int>(KEY_CURRENT_INDEX);
                      _categoryViewModel.CurrentBashDataIndex = Math.Min(index, _categoryViewModel.BashCount - 1);
                }
            }

            if (!success)
            {
                // TODO: handle error?
            }

            ShowBashInfoBar.Begin();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            PhoneStateHelper.SaveValue(KEY_CURRENT_INDEX, _categoryViewModel.CurrentBashDataIndex);

            // ensure favorites are saved (for my favorites page)
            _favoriteManager.SaveData();
        }

        private void QuoteItemLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var grid = sender as Grid;

            if (grid != null)
            {
                Storyboard storyboard;
                int offset = 0;
                if (_categoryViewModel.IsDataFreshlyLoaded)
                {
                    storyboard = grid.Resources["FadeInUp"] as Storyboard;
                    offset = 150;
                }
                else if (CategoryViewModel.WasLastNavigationNext)
                    storyboard = grid.Resources["FadeInRightToLeft"] as Storyboard;
                else
                    storyboard = grid.Resources["FadeInLeftToRight"] as Storyboard;
                
                var animationIndex = (int)grid.Tag;

                storyboard.BeginTime = (TimeSpan.FromMilliseconds(offset + 33 * animationIndex));
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
            ShowShareBar.Begin();
        }

        private void JumpToClicked(object sender, EventArgs e)
        {
            ShowJumpBar.Begin();
        }

        private void HideBarsTapHandler(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideShareBar.Begin();
            HideJumpBar.Begin();
        }

        private void JumpToTapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideJumpBar.Begin();
        }

        private void JumpToLostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            HideJumpBar.Begin();
        }

        private void AppBarStateChanged(object sender, ApplicationBarStateChangedEventArgs e)
        {
            if (e.IsMenuVisible)
            {
                HideJumpBar.Begin();
                HideShareBar.Begin();
            }

        }
    }
}