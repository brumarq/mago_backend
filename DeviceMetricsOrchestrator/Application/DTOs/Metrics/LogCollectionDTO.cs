namespace Application.DTOs.Metrics
{
    public class LogCollectionDTO : BaseDTO
    {
        /// <summary>
        /// Device unique identifier
        /// </summary>
        public int DeviceId { get; set; }
        /// <summary>
        /// LogCollectionType object
        /// </summary>
        public LogCollectionTypeDTO? LogCollectionType { get; set; }
    }
}
