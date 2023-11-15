using System;
namespace Model.DTOs
{
	public class BaseDTO
	{
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

