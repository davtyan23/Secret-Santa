using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Group
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; } = null!;
        public int OwnerUserID { get; set; } 
        public string? GroupDescription {  get; set; }
        public string? GroupLocation { get; set; }
        public int MinBudget { get; set; }
        public int MaxBudget { get; set; }
    }
}
