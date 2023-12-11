namespace Application.DTOs.Metrics
{
    public class LogCollectionDTO : BaseDTO
    {
        //public DeviceResponseDTO? Device { get; set; }
        public int DeviceId { get; set; }
        public LogCollectionTypeDTO? LogCollectionType { get; set; }
    }
}
