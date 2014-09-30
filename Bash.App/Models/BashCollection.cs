using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bash.App.Models
{
    [DataContract]
    public class BashCollection
    {
        [DataMember(Name = "Inhalte")]
        public BashDatas Contents { get; set; }

        [DataMember(Name = "Statistik")]
        public BashStatistic Statistic { get; set; }
    }
}
