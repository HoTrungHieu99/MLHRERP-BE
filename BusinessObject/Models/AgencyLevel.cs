using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class AgencyLevel
    {
        [Key]
        public long LevelId { get; set; }

        [Required]
        public string LevelName { get; set; }

        public decimal? DiscountPercentage { get; set; }
        public decimal? CreditLimit { get; set; }
        public int? PaymentTerm { get; set; }

        public ICollection<AgencyAccountLevel> AgencyAccountLevels { get; set; }
    }
}
