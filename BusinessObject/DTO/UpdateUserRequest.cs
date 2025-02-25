using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class UpdateUserRequest
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }

        // Chỉ dành cho Employee
        public string? FullName { get; set; }

        // Chỉ dành cho Agency
        public string? AgencyName { get; set; }

        // Địa chỉ chung
        public string? Street { get; set; }
        public string? WardName { get; set; }
        public string? DistrictName { get; set; }
        public string? ProvinceName { get; set; }
    }

}
