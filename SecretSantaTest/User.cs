using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaTest
{
    public class Users
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string number { get; set; }

        public override string ToString()
        {
            return $"{name} : {number}";
        }
    }
}


