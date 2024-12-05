using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class PassResetConfiramtionCode
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string ConfirmationCode { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
