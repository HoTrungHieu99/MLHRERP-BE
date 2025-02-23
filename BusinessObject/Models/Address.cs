using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BusinessObject.Models
{

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Address
    {
        [Key]
        public int AddressId { get; set; } // Mã địa chỉ

        [Required]
        public string Street { get; set; } // Tên đường

        public int WardId { get; set; }
        public int DistrictId { get; set; }
        public int ProvinceId { get; set; }

        [ForeignKey("WardId")]
        public Ward Ward { get; set; } // 👈 Liên kết với xã/phường

        [ForeignKey("DistrictId")]
        public District District { get; set; } // 👈 Liên kết với huyện/quận

        [ForeignKey("ProvinceId")]
        public Province Province { get; set; } // 👈 Liên kết với tỉnh/thành phố

        public ICollection<Employee> Employees { get; set; }
        public ICollection<AgencyAccount> AgencyAccounts { get; set; }
    }

}
