using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bash.App.Models
{
    [DataContract]
    public class BashData
    {
        private const string NEWLINE_DELEMITER = "[newline]<";
        private const string NEWLINE = "[newline]";

        public BashData()
        {
            Id = string.Empty;
        }

        [DataMember(Name="ident")]
        public string Id { get; set; }

        [DataMember(Name = "ts")]
        public string Timestamp { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "rating")]
        public string Rating { get; set; }

        public List<BashQuoteItem> QuoteItems
        {
            get
            {
                var result = new List<BashQuoteItem>();
                var persons = new Dictionary<string, int>();

                string[] splittedConversation = Content.Split(new string[] {NEWLINE_DELEMITER}, StringSplitOptions.RemoveEmptyEntries);

                foreach(var conversationPart in splittedConversation)
                {
                    BashQuoteItem item = new BashQuoteItem();

                    // nick
                    int nicEndIndex = conversationPart.IndexOf('>');
                    item.Nick = conversationPart.Substring(0, nicEndIndex).Replace("<", "");

                    // text
                    var quoteText = conversationPart.Substring(nicEndIndex + 1, conversationPart.Length - nicEndIndex - 1);
                    item.Text = quoteText.Replace(NEWLINE, "\n").Trim(); // TODO: parsing-probleme bei quotes, die "[newline]<" enthalten !!!

                    // person index
                    if (persons.ContainsKey(item.Nick))
                    {
                        item.PersonIndex = persons[item.Nick];
                    }
                    else
                    {
                        item.PersonIndex = persons.Count;
                        persons.Add(item.Nick, item.PersonIndex);
                    }

                    result.Add(item);
                }

                return result;
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (this.GetType() != obj.GetType())
                return false;
            var data = obj as BashData;

            return data.Id == this.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
