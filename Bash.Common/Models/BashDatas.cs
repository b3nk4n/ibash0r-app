using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bash.Common.Models
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
