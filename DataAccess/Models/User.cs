using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
    public DateTime RegisterTime { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AssignedRole> AssignedRoles { get; set; } = new List<AssignedRole>();

    public virtual ICollection<UserPass> UserPasses { get; set; } = new List<UserPass>();
}
