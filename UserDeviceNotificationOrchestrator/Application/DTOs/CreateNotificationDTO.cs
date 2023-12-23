using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateNotificationDTO
    {
        public int DeviceID { get; set; }
        public int StatusTypeID { get; set; }
        public string Message { get; set; }
    }
}
