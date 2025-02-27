using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class TaxConfig
    {
        [Key]
        public int TaxId { get; set; }

        [Required, MaxLength(255)]
        public string TaxName { get; set; }

        [Required]
        public decimal TaxRate { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
