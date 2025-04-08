using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AgencyLevel
{
    public class CurrentAgencyLevelDto
    {
        public string LevelName { get; set; }
        public decimal? CreditLimit { get; set; }
        public int? PaymentTerm { get; set; }
        public decimal? DiscountPercentage { get; set; }
    }

}
