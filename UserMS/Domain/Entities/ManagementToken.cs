using System.Text.Json.Serialization;

namespace Domain.Entities;

public class ManagementToken
{
    [JsonPropertyName("access_token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    public DateTime ExpirationTime { get; set; }
    
    
}