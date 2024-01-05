namespace Application.DTOs.Metrics
{
    public class FieldDTO : BaseDTO
    {
        /// <summary>
        /// Name of the field
        /// </summary>
        /// <example>Temperature</example>
        public string? Name { get; set; }
        /// <summary>
        /// Unit unique identifier
        /// </summary>
        public int UnitId { get; set; }
        /// <summary>
        /// DeviceType unique identifier
        /// </summary>
        public int DeviceTypeId { get; set; }
        /// <summary>
        /// Determines whether logging is possible
        /// </summary>
        public bool Loggable { get; set; }
    }
}
