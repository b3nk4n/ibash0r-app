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

namespace Bash.App.Pages
{
    public partial class MainPage : PhoneApplicationPage
    {
        IBashClient _bashClient = new BashClient();

        // Konstruktor
        public MainPage()
        {
            InitializeComponent();

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

            StartupActionManager.Instance.Fire(e);
        }
    }
}