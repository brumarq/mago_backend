namespace Domain.Entities;

public class Setting : BaseEntity
{
    public string? Name { get; set; }
    public float? DefaultValue { get; set; }
    public int UnitId { get; set; }
    public Unit? Unit { get; set; }
    public DeviceType? DeviceType { get; set; }
    public string? ViewedBy { get; set; }
    public string? EditedBy { get; set; }
}