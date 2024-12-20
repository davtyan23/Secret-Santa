﻿using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs.Request
{
    public class RoleAssignDTO
    {
        public int UserId { get; set; }
        public RoleIdEnum RoleId { get; set; }
    }
}
