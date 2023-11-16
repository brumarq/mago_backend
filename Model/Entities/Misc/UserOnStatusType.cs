using System.ComponentModel.DataAnnotations.Schema;
using Model.Entities;
using Model.Entities.Devices;
using Model.Entities.Users;

namespace Model;

public class UserOnStatusType : BaseEntity
{
	public User? User { get; set; }
	public Device? Device { get; set; }
	public StatusType? StatusType { get; set; }
}