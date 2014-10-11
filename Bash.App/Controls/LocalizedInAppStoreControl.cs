﻿using Bash.App.Resources;
using PhoneKit.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Bash.App.Controls
{
    public class LocalizedInAppStoreControl : MyInAppStoreControlBase
    {
        public LocalizedInAppStoreControl()
        {
            BackgroundTheme.Color = (Color)App.Current.Resources["ThemeColorBlue"];
        }

        /// <summary>
        /// Localizes the user control content and texts.
        /// </summary>
        protected override void LocalizeContent()
        {
            InAppStoreLoadingText = AppResources.InAppStoreLoading;
            InAppStoreNoProductsText = AppResources.InAppStoreNoProducts;
            InAppStorePurchasedText = AppResources.InAppStorePurchased;
            SupportedProductIds = AppConstants.IAP_AWESOME_EDITION;
        }
    }
}
