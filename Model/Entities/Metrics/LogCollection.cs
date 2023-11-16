using System.ComponentModel.DataAnnotations.Schema;
using Model.Entities;
using Model.Entities.Devices;

namespace Model;

public class LogCollection : BaseEntity
{
	public Device? Device { get; set; }
	public DateTime Timestamp { get; set; }
	public ICollection<LogValue>? Values { get; set; } = new List<LogValue>();
	public LogCollectionType? LogCollectionType { get; set; }
}
