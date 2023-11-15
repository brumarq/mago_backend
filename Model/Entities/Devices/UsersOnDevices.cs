using Model.Entities.Users;
using Model.Enums;

namespace Model.Entities.Devices
{
    public class UsersOnDevices : BaseEntity
    {
       public User? User { get; set; }
       public Device? Device { get; set; }
       public Role Role { get; set; }
       public bool ConnectionEmail { get; set; }
    }
}
