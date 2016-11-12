using System.Runtime.Serialization;

namespace Bash.Common.Models
{
    [DataContract]
    public class BashCollection
    {
        public BashCollection()
        {
            Contents = new BashDatas();
            Statistic = new BashStatistic();
        }

        [DataMember(Name = "Inhalte")]
        public BashDatas Contents { get; set; }

        [DataMember(Name = "Statistik")]
        public BashStatistic Statistic { get; set; }
    }
}
