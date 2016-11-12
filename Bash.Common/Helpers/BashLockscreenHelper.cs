using Bash.Common;
using Bash.Common.Controls;
using Bash.Common.Models;
using PhoneKit.Framework.Core.Graphics;
using PhoneKit.Framework.Core.LockScreen;
using PhoneKit.Framework.Core.Storage;
using System;
using System.Windows.Media.Imaging;

namespace Bash.App.Helpers
{
    public static class BashLockscreenHelper
    {
        private static Random random = new Random();

        public static void UpdateAsync(BashCollection data)
        {
            if (!LockScreenHelper.HasAccess())
                return;

            WriteableBitmap lockGfx;
            Uri lockUri;

            // get data
            if (data != null && data.Contents.Data.Count > 0)
            {
                // find quote
                int index = -1;
                for (int retry = 0; retry < AppConstants.QUOTES_COUNT_PRELOAD; retry++)
                {
                    int i = random.Next(data.Contents.Data.Count);

                    int heightScore = data.Contents.Data[i].GuessHeightScore();
                    if (heightScore > 0 && heightScore < 15)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1)
                    return;

                // render image
                lockGfx = GraphicsHelper.Create(
                    new LockQuoteControl(
                        data.Contents.Data[index],
                        Settings.LockScreenBackgroundColor.Value,
                        Settings.LockScreenBackgroundImagePath.Value,
                        Settings.LockScreenBackgroundImageOpacity.Value
                    ));

                // save lock image
                var nextExtension = DateTime.Now.Millisecond % 7;
                lockUri = StorageHelper.SaveJpeg(string.Format("/lockquote_{0}.jpg", nextExtension), lockGfx);

                // set lockscreen image
                LockScreenHelper.SetLockScreenImage(lockUri, true);
            }
        }
    }
}
