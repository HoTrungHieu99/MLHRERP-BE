using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class UpdateRequestDto
    {
        [Required]
        public long RequestId { get; set; }

        [Required]
        public string RequestStatus { get; set; } // PENDING, APPROVED, REJECTED

        [Required]
        public int Quantity { get; set; }
    }
}
