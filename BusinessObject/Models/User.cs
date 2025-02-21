using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace BusinessObject.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public long UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string UserType { get; set; } // EMPLOYEE or AGENT

        public string Phone { get; set; }
        public bool Status { get; set; } = false;

        public ICollection<UserRole> UserRoles { get; set; }
        public Employee Employee { get; set; }
        public AgencyAccount AgencyAccount { get; set; }
    }
}
