namespace Application.DTOs.Metrics
{
    public class FieldDTO : BaseDTO
    {
        public string? Name { get; set; }
        public int UnitId { get; set; }
        //public DeviceTypeResponseDTO? DeviceType { get; set; }
        public int DeviceTypeId { get; set; }
        public bool Loggable { get; set; }
    }
}
