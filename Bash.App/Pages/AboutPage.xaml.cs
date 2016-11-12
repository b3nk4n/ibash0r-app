using Microsoft.Phone.Controls;
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