﻿using Domain.Entities;

namespace Application.DTOs
{
    public class DeviceResponseDTO : BaseDTO
    {
        public string? Name { get; set; }
        public DeviceType? DeviceType { get; set; }
        public bool SendSettingsAtConn { get; set; }
        public bool SendSettingsNow { get; set; }
        public string? AuthId { get; set; }
    }
}