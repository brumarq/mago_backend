﻿namespace Application.DTOs.DeviceMetrics
{
    public class DeviceMetricsResponseDTO : BaseDTO
    {
        /// <summary>
        /// Value of the (current) LogValue
        /// </summary>
        public float? Value { get; set; }
        /// <summary>
        /// Field object
        /// </summary>
        public FieldResponseDTO? Field { get; set; }
        /// <summary>
        /// LogCollection object
        /// </summary>
        public LogCollectionResponseDTO? LogCollection { get; set; }
    }
}
