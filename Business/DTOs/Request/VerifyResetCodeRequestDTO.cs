﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs.Request
{
    public class VerifyResetCodeRequestDTO
    {
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
    }
}
