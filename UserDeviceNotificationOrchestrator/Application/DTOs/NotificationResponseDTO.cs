﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class NotificationResponseDTO
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public int StatusTypeId { get; set; }
        public string? Message { get; set; }
        public int DeviceID { get; set; }
    }
}
