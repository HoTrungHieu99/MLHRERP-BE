using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace BusinessObject.Models
{
    public class Ward
    {
        [Key]
        [JsonProperty("code")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int WardId { get; set; }

        [JsonProperty("name")]
        public string WardName { get; set; }

        [JsonProperty("division_type")]
        public string WardType { get; set; }

        [JsonProperty("codename")]
        public string CodeName { get; set; }

        [JsonProperty("district_code")]
        public int DistrictId { get; set; }

        [ForeignKey("DistrictId")]
        public District District { get; set; }

        public List<Address> Addresses { get; set; } = new List<Address>(); // ✅ Liên kết với Address
    }


}
