using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bash.App.Models
{
    [DataContract]
    public class BashDatas
    {
        public BashDatas()
        {
            Data = new List<BashData>();
        }

        [DataMember(Name = "last_page")]
        public int LastPage { get; set; }

        [DataMember(Name = "data")]
        public List<BashData> Data { get; set; }
    }
}
