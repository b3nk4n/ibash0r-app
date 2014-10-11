using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using PhoneKit.Framework.Core.MVVM;
using System.Windows.Input;

namespace Bash.Common.Models
{
    public class BashQuoteItem
    {
        private DelegateCommand<string> _copyPartToClipboardCommand;

        public  BashQuoteItem()
        {
            _copyPartToClipboardCommand = new DelegateCommand<string>((param) =>
            {
                Clipboard.SetText(param);
            },
            (param) =>
            {
                return true;
            });
        }

        public string Nick { get; set; }

        public string Text { get; set; }

        public int PersonIndex { get; set; }

        /// <summary>
        /// The index position used by the animation.
        /// </summary>
        public int IndexPosition { get; set; }

        public ICommand CopyPartToClipboardCommand
        {
            get { return _copyPartToClipboardCommand; }
        }
    }
}
