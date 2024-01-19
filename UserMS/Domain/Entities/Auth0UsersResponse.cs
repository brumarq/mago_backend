using System.Text.Json.Serialization;
using Application.DTOs;

namespace Domain.Entities;

public class Auth0UsersResponse
{
    [JsonPropertyName("users")]
    public List<UserCompressed> Users { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }
}