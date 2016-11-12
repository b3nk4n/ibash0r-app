using System.Runtime.Serialization;

namespace Bash.Common.Models
{
    [DataContract]
    public class BashStatistic
    {
        [DataMember(Name = "all")]
        public string All { get; set; }

        [DataMember(Name = "today")]
        public string Today { get; set; }

        [DataMember(Name = "votes")]
        public string Votes { get; set; }

        [DataMember(Name = "comments")]
        public string Comments { get; set; }
    }
}
