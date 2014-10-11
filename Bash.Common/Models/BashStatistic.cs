using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
