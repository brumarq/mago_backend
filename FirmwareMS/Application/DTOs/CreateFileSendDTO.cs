﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateFileSendDTO
    {
        public int DeviceId { get; set; }
        public string? File { get; set; }
    }
}