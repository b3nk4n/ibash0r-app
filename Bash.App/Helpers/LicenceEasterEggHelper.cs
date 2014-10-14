using Bash.App.Resources;
using Bash.Common;
using PhoneKit.Framework.Core.Storage;
using PhoneKit.Framework.InAppPurchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bash.App.Helpers
{
    public static class LicenceEasterEggHelper
    {
        private static StoredObject<int> easterEggCounter = new StoredObject<int>("__easterEgg", 0);
        private const int EASTER_EGG_LIMIT = 33;

        public static bool IsAwesomeEditionUnlocked()
        {
            return InAppPurchaseHelper.IsProductActive(AppConstants.IAP_AWESOME_EDITION) ||
                easterEggCounter.Value >= EASTER_EGG_LIMIT;
        }

        public static void TriggerEasterEggRequested()
        {
            easterEggCounter.Value++;
            if (easterEggCounter.Value == EASTER_EGG_LIMIT)
            {
                MessageBox.Show(AppResources.EasterEggMessage, AppResources.EasterEggTitle, MessageBoxButton.OK);
            }
        }
    }
}
