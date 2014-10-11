using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Bash.App.Resources;
using Bash.App.Data;
using Bash.App.Models;
using Newtonsoft.Json;
using PhoneKit.Framework.Support;
using Bash.App.ViewModels;
using Ninject;
using System.Windows.Input;
using System.Windows.Data;

namespace Bash.App.Pages
{
    public partial class MainPage : PhoneApplicationPage
    {
        IBashClient _bashClient = new BashClient();

        IMainViewModel _mainViewModel;

        // Konstruktor
        public MainPage()
        {
            InitializeComponent();

            _mainViewModel = App.Injector.Get<IMainViewModel>();

            // register startup actions
            StartupActionManager.Instance.Register(10, ActionExecutionRule.Equals, () =>
            {
                FeedbackManager.Instance.StartFirst();
            });
            StartupActionManager.Instance.Register(20, ActionExecutionRule.Equals, () =>
            {
                FeedbackManager.Instance.StartSecond();
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // setup view model
            _mainViewModel.NavigationService = NavigationService;
            DataContext = _mainViewModel;

            StartupActionManager.Instance.Fire(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                StartupAnimation.Begin();
            }
        }

        private void TileTapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var tile = sender as HyperlinkButton;

            if (tile != null)
            {
                var tag = (string)tile.Tag;
                var link = tag.Split(';')[1];
                TurnstileFeatherEffect.SetFeatheringIndex(tile, -1);
                NavigationService.Navigate(new Uri(link, UriKind.Relative));
            }
        }

        private void TileLoaded(object sender, RoutedEventArgs e)
        {
            var tile = sender as HyperlinkButton;

            if (tile != null)
            {
                var tag = (string)tile.Tag;
                var animationId = int.Parse(tag.Split(';')[0]);
                TurnstileFeatherEffect.SetFeatheringIndex(tile,animationId);
            }
        }

        private void SearchKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_mainViewModel.SearchCommand.CanExecute(null))
                {
                    _mainViewModel.SearchCommand.Execute(null);
                }
            }
        }

        private void SearchTextChangedEvent(object sender, TextChangedEventArgs e)
        {
            TextBox txtbox = sender as TextBox;
            _mainViewModel.SearchTerm = txtbox.Text;
        }
    }
}