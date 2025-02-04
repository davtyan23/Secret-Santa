using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    [Table("GroupInfo")]
    public class GroupInfo
    {
        public int GroupInfoID { get; set; }
        public int UserGroupID { get; set; }
        public int RecieverID { get; set; }
        public string? Whishlist { get; set; }
        public UserGroup UserGroups { get; set; } = null!;
        public User Reciever { get; set; } = null!;
    }
}
