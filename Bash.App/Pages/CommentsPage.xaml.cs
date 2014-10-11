using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Bash.App.ViewModels;
using Ninject;
using Bash.App.Data;
using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Bash.App.Pages
{
    public partial class CommentsPage : PhoneApplicationPage
    {
        private ICommentsViewModel _commentsViewModel;

        public CommentsPage()
        {
            InitializeComponent();
            _commentsViewModel = App.Injector.Get<ICommentsViewModel>();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // setup view model
            _commentsViewModel.Reset();
            DataContext = _commentsViewModel;

            bool success = false;
            if (NavigationContext.QueryString.ContainsKey(AppConstants.PARAM_ID))
            {
                var id = int.Parse(NavigationContext.QueryString[AppConstants.PARAM_ID]);
                await _commentsViewModel.LoadCommentsAsync(id);
            }
            else
            {
                throw new ArgumentException("Parameter required.");
            }

            if (!success)
            {
                // TODO: handle error?
            }
        }

        private void CommentItemLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var grid = sender as Grid;

            if (grid != null)
            {
                var animationIndex = (int)grid.Tag;
                if (animationIndex < 15)
                {
                    Storyboard storyboard = grid.Resources["FadeInUp"] as Storyboard;
                    storyboard.BeginTime = (TimeSpan.FromMilliseconds(5 + 25 * animationIndex));
                    storyboard.Begin();
                }
                else
                {
                    grid.Opacity = 1.0;
                }
            }
        }
    }
}