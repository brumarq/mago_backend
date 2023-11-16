using System.ComponentModel.DataAnnotations.Schema;
using Model.Entities.Users;
using Model.Enums;

namespace Model.Entities.Devices
{
    public class UsersOnDevices : BaseEntity
    {
        public Role Role { get; set; }
        public bool ConnectionEmail { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("DeviceId")]
        public Device? Device { get; set; }
    }
}
