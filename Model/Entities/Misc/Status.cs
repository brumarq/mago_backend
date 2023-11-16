using System.ComponentModel.DataAnnotations.Schema;
using Model.Entities;
using Model.Entities.Devices;

namespace Model;

public class Status : BaseEntity
{
	public DateTime TimeStamp { get; set; }
	public string? Message { get; set; }

	[ForeignKey("StatusTypeId")]
	public StatusType? StatusType { get; set; }

	[ForeignKey("DeviceId")]
	public Device? Device { get; set; }
}
