using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Role
{
    public RoleIdEnum RoleId { get; set; }

    public string RoleName { get; set; } = null!;

   // public virtual ICollection<AssignedRole> AssignedRoles { get; set; } = new List<AssignedRole>();
   // public ICollection<Groups> UserRolesGroups { get; set; }
}
