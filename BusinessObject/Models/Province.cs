using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{


    public class Province
    {
        [Key]
        [JsonProperty("code")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProvinceId { get; set; }

        [JsonProperty("name")]
        public string ProvinceName { get; set; }

        [JsonProperty("division_type")]
        public string ProvinceType { get; set; }

        [JsonProperty("codename")]
        public string CodeName { get; set; }

        [JsonProperty("phone_code")]
        public int PhoneCode { get; set; }

        public List<District> Districts { get; set; } = new List<District>();
        public List<Address> Addresses { get; set; } = new List<Address>(); // ✅ Liên kết với Address
    }



}
