using System.Text.Json.Serialization;
using Domain.Entities;

namespace Application.DTOs;

public class UserCompressed
{
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("picture")]
    public string? Picture { get; set; }
}