using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class Image
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; }

        // Khóa ngoại liên kết với Product
        public long ProductId { get; set; }
        public Product Product { get; set; }
    }

}
