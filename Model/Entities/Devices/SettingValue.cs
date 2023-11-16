using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities.Users;

namespace Model.Entities.Devices
{
    public class SettingValue
    {
        public double Value { get; set; }
        public UpdateStatus UpdateStatus { get; set; }

        [ForeignKey("DeviceId")]
        public Device? Device { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("SettingId")]
        public Setting? Setting { get; set; }
    }
}
