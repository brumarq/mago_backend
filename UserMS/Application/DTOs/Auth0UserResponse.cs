using System.Text.Json.Serialization;

namespace Application.DTOs;

public abstract class Auth0UserResponse
{
    
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }
    
    [JsonPropertyName("family_name")]
    public string? FamilyName { get; set; }
    
    [JsonPropertyName("given_name")]
    public string? GivenName { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }
    
    [JsonPropertyName("blocked")]
    public bool Blocked { get; set; }

    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public string? UpdatedAt { get; set; }

    [JsonPropertyName("picture")]
    public string? Picture { get; set; }


}
