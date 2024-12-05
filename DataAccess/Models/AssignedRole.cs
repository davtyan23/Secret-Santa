using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class AssignedRole
{
    public int UserRolesId { get; set; }

    public int UserId { get; set; }

    public int RoleId { get; set; }

}
