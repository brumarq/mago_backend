using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UsersOnDevices
{
    public class CreateUserOnDeviceDTO : BaseDTO
    {
        public string? UserId { get; set; }
        public int DeviceId { get; set; }
        public string? Role { get; set; }
        public bool ConnectionMail { get; set; }    
    }
}
