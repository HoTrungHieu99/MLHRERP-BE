using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class UpdateEmployeeRequest
    {
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public int LocationId { get; set; }
    }
}
