using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    public class Permission
    {
        [Key]
        public long PermissionId { get; set; }

        [Required]
        public string PermissionName { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
