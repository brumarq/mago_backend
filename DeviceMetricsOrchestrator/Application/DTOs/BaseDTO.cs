namespace Application.DTOs;
public class BaseDTO
{
    /// <summary>
    /// Base entity identifier used by child classes
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Bases entity for knowing when an entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// Bases entity for knowing when an entity was updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } 
}