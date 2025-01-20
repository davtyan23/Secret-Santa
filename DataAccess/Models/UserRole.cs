using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class UserRole
    {
        public int UserRolesID { get; set; }
        public RoleIdEnum RoleID { get; set; }
        public int UserID { get; set; } 
        public int GroupID {  get; set; }
        public virtual User? User { get; set; } 
        public virtual Role? Role { get; set; }
    }
}
