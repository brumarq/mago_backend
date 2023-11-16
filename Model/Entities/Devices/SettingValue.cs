using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities.Users;

namespace Model.Entities.Devices
{
    public class SettingValue : BaseEntity
    {
        public float Value { get; set; }
        public Setting? Setting { get; set; }
        public string? UpdateStatus { get; set; }
        public Device? Device { get; set; }
        public User? User { get; set; }
    }
}
