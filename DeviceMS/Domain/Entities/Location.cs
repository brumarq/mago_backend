namespace Domain.Entities;

public class Location : BaseEntity
{
    public string? Name { get; set; }
    public string? Street { get; set; }
    public string? Zip { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}