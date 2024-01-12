using TestDomain.Metrics.Device;

namespace TestDomain.Metrics
{
    public class LogCollectionResponseDTO : BaseDTO
    {
        /// <summary>
        /// Device object
        /// </summary>
        public DeviceResponseDTO? Device { get; set; }
        /// <summary>
        /// LogCollectionType object
        /// </summary>
        public LogCollectionTypeDTO? LogCollectionType { get; set; }
    }

}
