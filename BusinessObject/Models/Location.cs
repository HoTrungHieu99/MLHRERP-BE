using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }

        [Required]
        public string CityName { get; set; }

        public ICollection<Employee> Employees { get; set; }
        public ICollection<AgencyAccount> AgencyAccounts { get; set; }
        // sua thanh 4 bang moi( tinh, huyen, xa, dia chi-duong va so nha) 
    }
}
