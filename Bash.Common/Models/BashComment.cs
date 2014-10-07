using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bash.App.Models
{
    [DataContract]
    public class BashComment
    {
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
                return _text.Replace("[newline]", "\n");
            } 
            set
            {
                _text = value;
            }
        }
    }
}
