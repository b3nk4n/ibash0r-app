using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bash.App.Models
{
    [DataContract]
    public class BashComments
    {
        [DataMember(Name = "comments")]
        public List<BashComment> Comments { get; set; }
    }
}
