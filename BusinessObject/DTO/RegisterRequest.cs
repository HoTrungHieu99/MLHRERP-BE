using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string UserType { get; set; } // "Agent" hoặc "Employee"

        // Chỉ dành cho Employee
        public string FullName { get; set; }
        /*public string Position { get; set; }
        public string Department { get; set; }*/
        public int LocationId { get; set; }

        // Chỉ dành cho Agent
        public string AgencyName { get; set; }
        //public string Address { get; set; }
    }

}
