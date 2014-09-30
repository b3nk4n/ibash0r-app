using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Bash.App.ViewModels;
using Ninject;
using Bash.App.Data;
using System;

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
            DataContext = _commentsViewModel;

            bool success = false;
            if (NavigationContext.QueryString.ContainsKey(BashClient.PARAM_ID))
            {
                var id = NavigationContext.QueryString[BashClient.PARAM_ID];
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
    }
}