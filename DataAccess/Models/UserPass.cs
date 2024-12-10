using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class UserPass
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Email { get; set; } = null!;

    public string PassHash { get; set; } = null!;

    //public string Role {  get; set; } = null!;

    //public virtual User User { get; set; } = null!;
    
}
