using BusinessObject.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    public class Employee
    {
        [Key]
        public long EmployeeId { get; set; }

        [Required]
        public string FullName { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public string Department { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public int AddressId { get; set; }
        [ForeignKey("AddressId")]
        public Address Address { get; set; }
    }
}
