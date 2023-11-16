using System.ComponentModel.DataAnnotations.Schema;
using Model.Entities;
using Model.Entities.Devices;
using Model.Entities.Users;

namespace Model;

public class UserOnStatusType : BaseEntity
{
	[ForeignKey("UserId")]
	public User? User { get; set; }

	[ForeignKey("DeviceId")]
	public Device? Device { get; set; }

	[ForeignKey("StatusTypeId")]
	public StatusType? StatusType { get; set; }
}