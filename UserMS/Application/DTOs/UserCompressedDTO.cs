using System.Text.Json.Serialization;
using Domain.Entities;

namespace Application.DTOs;

public class UserCompressedDTO
{
    public UserCompressed User { get; set; }
    public string Role { get; set; }
}