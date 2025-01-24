using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class UserGroup
    {
        public int UserGroupID { get; set; }
        public int UserID { get; set; }
        public int GroupID { get; set; }
        public User User { get; set; } = null!;
        public Group Groups { get; set; } = null!;
    }
}
