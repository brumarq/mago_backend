namespace Domain.Entities;

public class LogValue : BaseEntity
{
    public float? Value { get; set; }
    public Field? Field { get; set; }
    public LogCollection? Collection { get; set; }
}