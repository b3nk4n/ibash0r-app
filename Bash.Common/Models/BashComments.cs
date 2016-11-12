using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bash.Common.Models
{
    [DataContract]
    public class BashComments
    {
        [DataMember(Name = "comments")]
        public List<BashComment> Comments { get; set; }
    }
}
