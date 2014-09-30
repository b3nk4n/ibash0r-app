﻿using System;
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
        }
    }
}