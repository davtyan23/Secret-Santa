using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs
{
    public class UserPassDto
    {
        public string Email {  get; set; }
        public string PassHash { get; set; }
    }
}
