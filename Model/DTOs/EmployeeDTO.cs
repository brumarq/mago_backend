using System;
namespace Model.DTOs
{
	public class EmployeeDTO : BaseDTO
	{
        public int EmployeeNumber { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
    }
}

