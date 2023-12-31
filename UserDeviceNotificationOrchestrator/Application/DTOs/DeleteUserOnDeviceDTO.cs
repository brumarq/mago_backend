using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class DeleteUserOnDeviceDTO
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int DeviceId { get; set; }
    }
}
