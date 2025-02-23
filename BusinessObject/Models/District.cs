using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BusinessObject.Models
{

    public class District
    {
        [Key]
        [JsonProperty("code")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DistrictId { get; set; }

        [JsonProperty("name")]
        public string DistrictName { get; set; }

        [JsonProperty("division_type")]
        public string DistrictType { get; set; }

        [JsonProperty("codename")]
        public string CodeName { get; set; }

        [JsonProperty("province_code")]
        public int ProvinceId { get; set; }

        [ForeignKey("ProvinceId")]
        public Province Province { get; set; }

        public List<Ward> Wards { get; set; } = new List<Ward>();
        public List<Address> Addresses { get; set; } = new List<Address>(); // ✅ Liên kết với Address
    }



}
