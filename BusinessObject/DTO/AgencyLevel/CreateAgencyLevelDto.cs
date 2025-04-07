using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.AgencyLevel
{
    public class CreateAgencyLevelDto
    {
        [Required]
        public string LevelName { get; set; }

        [Range(0, 100)]
        public decimal? DiscountPercentage { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? CreditLimit { get; set; }

        [Range(0, 365)]
        public int? PaymentTerm { get; set; }
    }

}
