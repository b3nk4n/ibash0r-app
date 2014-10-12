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
                //Clipboard.SetText(param);
                // moved to click event, because this call is not allowed in a Common project!
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

        public int HeightScore { get; set; }
    }
}
