namespace Drive_Smart_2._0.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string EmployeeID { get; set; }
        public string FullName { get; set; }
        public string NIC { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Position { get; set; }      // Admin, Manager, Intern
        public decimal Salary { get; set; }
        public DateTime JoiningDate { get; set; }
        public string Address { get; set; }

        // Auth fields
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool IsApproved { get; set; } = true;
        public bool MustChangePassword { get; set; } = true;
    }
}