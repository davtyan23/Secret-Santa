using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class GroupInfo
    {
        public int GroupInfoID { get; set; }
        public int UserGroupID { get; set; }
        public int RecieverID { get; set; }
        public string Whishlist { get; set; }
        public UserGroup UserGroups { get; set; } = null!;
        public User Reciever { get; set; } = null!;
    }
}
