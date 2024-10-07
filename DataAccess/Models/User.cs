using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class User
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Number { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<AssignedRole> AssignedRoles { get; set; } = new List<AssignedRole>();

    public virtual ICollection<UserPass> UserPasses { get; set; } = new List<UserPass>();
}
