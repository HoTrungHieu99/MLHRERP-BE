using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    public class RolePermission
    {
        [Key]
        public long RolePermissionId { get; set; }

        public long RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        public long PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public Permission Permission { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
