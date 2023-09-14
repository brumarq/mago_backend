using System;
namespace Model.Entities
{
	public class Employee : BaseEntity
	{
        public int EmployeeNumber { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
    }
}

