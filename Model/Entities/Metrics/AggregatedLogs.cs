using Model.Entities;

namespace Model;

public class AggregatedLogs : BaseEntity
{
	public string? Type { get; set; }
	public DateOnly Date { get; set; }
	public double Average_Value { get; set; }
	public double Min_Value { get; set; }
	public double Max_Value { get; set; }
	public DateTime Last_Updated { get; set; }
}
