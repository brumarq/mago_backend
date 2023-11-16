using System.ComponentModel.DataAnnotations.Schema;
using Model.Entities;
using Model.Entities.Devices;

namespace Model;

public class Status : BaseEntity
{
	public DateTime Timestamp { get; set; }
	public StatusType? StatusType { get; set; }
	public string? Message { get; set; }
	public Device? Device { get; set; }
}
