using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Product
{
    public class TaxConfigDTO
    {
        public int TaxId { get; set; }
        public string TaxName { get; set; }
        public decimal TaxRate { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
    }

}
