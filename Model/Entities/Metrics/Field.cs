using System.ComponentModel.DataAnnotations.Schema;
using Model.Entities;
using Model.Entities.Devices;

namespace Model;

public class Field : BaseEntity
{
	public string? Name { get; set; }
	public bool Loggable { get; set; }

	[ForeignKey("UnitId")]
	public Unit? Unit { get; set; }

	[ForeignKey("DeviceTypeId")]
	public DeviceType? DeviceType { get; set; }
}
