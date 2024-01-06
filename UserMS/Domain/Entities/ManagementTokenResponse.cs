using System.Text.Json.Serialization;

namespace Domain.Entities;

public class ManagementTokenResponse
{
    [JsonPropertyName("access_token")]
    public string? Token { get; set; } = string.Empty;
}