using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class RegisterAccount
    {
        [Key] // Định nghĩa khóa chính
        public int RegisterId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public string UserType { get; set; } // "Agent" hoặc "Employee"

        // Chỉ dành cho Employee
        public string? FullName { get; set; }
        public string? Position { get; set; }
        public string? Department { get; set; }
        public int? LocationId { get; set; } // Cho phép null nếu là Agent

        // Chỉ dành cho Agent
        public string? AgencyName { get; set; }
        public string Address { get; set; }

        // Cột kiểm tra tài khoản đã được duyệt hay chưa
        public bool IsApproved { get; set; } = false;

        public DateTime CreatedAt { get; set; }
    }

}
