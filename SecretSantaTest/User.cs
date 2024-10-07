using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class User
    {
        public required string name { get; set; }
        public required string surname { get; set; }
        public required string number { get; set; }

        public override string ToString()
        {
            return $"{name} : {number}";
        }
    }
}


