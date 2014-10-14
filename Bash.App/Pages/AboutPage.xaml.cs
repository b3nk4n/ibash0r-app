using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Bash.App.Helpers;

namespace Bash.App.Pages
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void EasterEggDoubleTapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LicenceEasterEggHelper.TriggerEasterEggRequested();
        }
    }
}