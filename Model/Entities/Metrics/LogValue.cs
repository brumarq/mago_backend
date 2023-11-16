using System.ComponentModel.DataAnnotations.Schema;
using Model.Entities;

namespace Model;

public class LogValue : BaseEntity
{
	public double Value { get; set; }

	[ForeignKey("FieldId")]
	public Field? Field { get; set; }

	[ForeignKey("LogCollectionId")]
	public LogCollection? LogCollection { get; set; }
}
