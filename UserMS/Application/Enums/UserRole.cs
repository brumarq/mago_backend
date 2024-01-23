using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Application.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum UserRole
{
    Client,
    Admin
}