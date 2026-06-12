using Microsoft.EntityFrameworkCore;
using Drive_Smart_2._0.Auth.Models;

namespace Drive_Smart_2._0.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=EmployeeDB.db");
        }
    }
}