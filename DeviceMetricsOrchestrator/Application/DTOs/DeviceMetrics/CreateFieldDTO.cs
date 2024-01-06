namespace Application.DTOs.DeviceMetrics
{
    public class CreateFieldDTO
    {
        public string? Name { get; set; }
        public int UnitId { get; set; }
        public int DeviceTypeId { get; set; }
        public bool Loggable { get; set; }
    }
}
