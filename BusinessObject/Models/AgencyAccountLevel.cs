using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class AgencyAccountLevel
    {
        [Key]
        
        public long AgencyLevelId { get; set; }

        public decimal TotalDebtValue { get; set; }
        public decimal OrderDiscount { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal OrderRevenue { get; set; }
        public DateTime ChangeDate { get; set; } = DateTime.Now;

        public long AgencyId { get; set; }
        [ForeignKey("AgencyId")]
        public AgencyAccount Agency { get; set; }

        public long LevelId { get; set; }
        [ForeignKey("LevelId")]
        public AgencyLevel Level { get; set; }
    }
}
