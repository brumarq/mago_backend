using System.ComponentModel.DataAnnotations.Schema;
using Model.Entities;
using Model.Entities.Devices;

namespace Model;

public class LogCollection : BaseEntity
{
	public DateTime TimeStamp { get; set; }

	[ForeignKey("DeviceId")]
	public Device? Device { get; set; }

	[ForeignKey("LogCollectionTypeId")]
	public LogCollectionType? LogCollectionType { get; set; }
}
