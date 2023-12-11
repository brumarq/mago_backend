using System.Text.Json.Serialization;

namespace Application.DTOs;

public class ManagementTokenResponse
{
    [JsonPropertyName("access_token")]
    public string Token { get; set; } = string.Empty;
}