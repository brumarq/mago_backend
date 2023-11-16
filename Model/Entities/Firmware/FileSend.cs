using System.ComponentModel.DataAnnotations.Schema;
using Model.Entities;
using Model.Entities.Devices;
using Model.Entities.Users;

namespace Model;

public class FileSend : BaseEntity
{
	public UpdateStatus UpdateStatus { get; set; }
	public string? File { get; set; }
	public int CurrPart { get; set; }
	public int TotParts { get; set; }

	[ForeignKey("DeviceId")]
	public Device? Device { get; set; }

	[ForeignKey("UserId")]
	public User? User { get; set; }
}
