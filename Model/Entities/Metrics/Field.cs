using System.ComponentModel.DataAnnotations.Schema;
using Model.Entities;
using Model.Entities.Devices;

namespace Model;

public class Field : BaseEntity
{
	public string? Name { get; set; }
	public Unit? Unit { get; set; }
	public DeviceType? DeviceType { get; set; }
	public bool Loggable { get; set; }
	public ICollection<LogValue>? LogValues { get; set; } = new HashSet<LogValue>();
}
