using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using PhoneKit.Framework.Core.MVVM;

namespace Bash.Common.Models
{
    [DataContract]
    public class BashData : ViewModelBase
    {
        private static readonly string[] NEWLINE_DELEMITERS = { "[newline]<", "[newline] <" };
        private const string NEWLINE = "[newline]";

        private static readonly string[] CUTSOM_SEPERATORS = { "---- 1 Stunde später ----" };

        private List<BashQuoteItem> _cachedQuoteItems;

        public BashData()
        {
        }

        [DataMember(Name = "ident")]
        public int Id { get; set; }

        [DataMember(Name = "ts")]
        public string Timestamp { get; set; }

        public string ShortTimestamp
        {
            get
            {
                return Timestamp.Split(' ')[0];
            }
        }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        private int _rating;

        [DataMember(Name = "rating")]
        public int Rating
        {
            get
            {
                return _rating;
            }
            set
            {
                if (_rating != value)
                {
                    _rating = value;
                    NotifyPropertyChanged("Rating");
                }
            }
        }

        public List<BashQuoteItem> QuoteItems
        {
            get
            {
                if (_cachedQuoteItems != null)
                    return _cachedQuoteItems;

                var result = new List<BashQuoteItem>();
                var persons = new Dictionary<string, int>();

                string[] splittedConversation = Content.Split(new string[]{ NEWLINE }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var conversationPart in splittedConversation)
                {
                    string nick;
                    int personIndex;
                    string text;
                    int nameOpen = conversationPart.IndexOf('<');
                    int nameClose = conversationPart.IndexOf('>');
                    int heightScore;

                    if (nameOpen != -1 && nameClose != -1)
                    {
                        nick = conversationPart.Substring(nameOpen + 1, nameClose - nameOpen - 1);
                        text = conversationPart.Substring(nameClose + 1, conversationPart.Length - nameClose - 1).Trim();

                        if (persons.ContainsKey(nick))
                        {
                            personIndex = persons[nick];
                        }
                        else
                        {
                            personIndex = persons.Count;
                            persons.Add(nick, personIndex);
                        }
                        heightScore = 3;
                    }
                    else if (IsServerText(conversationPart))
                    {
                        nick = "server";
                        personIndex = -1;
                        text = TrimServerText(conversationPart);
                        heightScore = 2;
                    }
                    else // belongs to the quote before
                    {
                        if (result.Count > 0)
                        {
                            var itemBefore = result[result.Count - 1];
                            itemBefore.Text += '\n' + conversationPart;
                            itemBefore.HeightScore++;
                        }
                        continue;
                    }

                    result.Add(new BashQuoteItem
                    {
                        Nick = nick,
                        PersonIndex = personIndex,
                        Text = text,
                        IndexPosition = result.Count,
                        HeightScore = heightScore
                    });
                }

                _cachedQuoteItems = result;
                return result;
            }
        }

        public int GuessHeightScore()
        {
            int heightScore = 0;

            foreach (var item in QuoteItems)
            {
                heightScore += item.HeightScore;
            }

            return heightScore;
        }

        private string TrimServerText(string text)
        {
            if (text.StartsWith("*** ") || text.StartsWith("<-- ") || text.StartsWith("--> "))
            {
                return text.Substring(4, text.Length - 4);
            }
            else if (text.StartsWith("* "))
            {
                return text.Substring(2, text.Length - 2);
            }

            return text;
        }

        private bool IsServerText(string text)
        {
            return (text.StartsWith("*") && (text.Contains("was banned from the server") || text.Contains("is back from") || text.Contains("was kicked by") || text.Contains("Quits: ") || text.Contains("Joins: ") || text.Contains("has joined") || text.Contains("has quit (") || text.Contains("changed nick to")) ||
                (text.StartsWith("<--") || (text.StartsWith("-->")) && (text.Contains("has quit (") || text.Contains(") has joined"))) ||
                text.Contains("has quit IRC") ||
                text.Equals("---- 1 Stunde später ----"));
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

        public string QuoteString {
            get
            {
                var sb = new StringBuilder();
                bool isNotFirst = false;
                foreach(var quote in QuoteItems)
                {
                    if (isNotFirst)
                        sb.Append('\n');
                    else
                        isNotFirst = true;

                    if (quote.PersonIndex == -1)
                        sb.Append(string.Format("*** {0}", quote.Text));
                    else
                    sb.Append(string.Format("<{0}> {1}", quote.Nick, quote.Text));
                }
                return sb.ToString();
            }
        }

        public Uri Uri
        {
            get
            {
                return new Uri(string.Format(@"http://www.ibash.de/zitat_{0}.html", Id), UriKind.Absolute);
            }
        }
    }
}
