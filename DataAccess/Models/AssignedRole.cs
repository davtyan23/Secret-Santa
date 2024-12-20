using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class AssignedRole
{
    public int UserRolesId { get; set; }

    public int UserId { get; set; }

    public RoleIdEnum RoleId { get; set; }

}

public enum RoleIdEnum
{
    Admin = 1,
    Participant = 2
}