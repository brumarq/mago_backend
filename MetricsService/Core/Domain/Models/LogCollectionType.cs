namespace MetricsService.Core.Domain.Models;

public class LogCollectionType : BaseEntity
{
	public ICollection<LogCollection>? Collections { get; set; } = new List<LogCollection>();
}
