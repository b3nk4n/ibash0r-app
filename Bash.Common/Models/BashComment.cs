using PhoneKit.Framework.Core.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bash.App.Models
{
    [DataContract]
    public class BashComment
    {
        private DelegateCommand<string> _copyPartToClipboardCommand;

        public BashComment()
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

        [DataMember(Name = "nick")]
        public string Nick { get; set; }

        [DataMember(Name = "ts")]
        public string Timestamp { get; set; }

        private string _text;

        [DataMember(Name = "text")]
        public string Text
        {
            get
            {
                if (_text == null)
                    return string.Empty;

                var replacedText = _text.Replace("[newline]", "\n");

                for (int i = 0; i < 3; ++i)
                {
                    replacedText = replacedText.Replace("\n\n\n\n", "\n");
                }

                return replacedText;
            } 
            set
            {
                _text = value;
            }
        }

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
