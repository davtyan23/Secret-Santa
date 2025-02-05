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

        [Column("RecieverID")]
        public int ReceiverID { get; set; }
        public string? Whishlist { get; set; }
        public UserGroup UserGroups { get; set; } = null!;
        public User Receiver { get; set; } = null!;

    }
}
