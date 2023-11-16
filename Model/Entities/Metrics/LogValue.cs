using System.ComponentModel.DataAnnotations.Schema;
using Model.Entities;

namespace Model;

public class LogValue : BaseEntity
{
	public float? Value { get; set; }
	public Field? Field { get; set; }
	public LogCollection? Collection { get; set; }
}
