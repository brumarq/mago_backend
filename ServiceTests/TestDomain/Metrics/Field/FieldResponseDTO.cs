﻿using TestDomain.Metrics.Device;
using TestDomain.Metrics.Unit;

namespace TestDomain.Metrics.Field
{
    public class FieldResponseDTO : BaseDTO
    {
        /// <summary>
        /// Name of the field
        /// </summary>
        /// <example>Temperature</example>
        public string? Name { get; set; }
        /// <summary>
        /// Unit object
        /// </summary>
        public UnitResponseDTO? Unit { get; set; }
        /// <summary>
        /// Device type object
        /// </summary>
        public DeviceTypeResponseDTO? DeviceType { get; set; }
        /// <summary>
        /// Determines whether the record is loggable of not
        /// </summary>
        public bool Loggable { get; set; }
    }

}
