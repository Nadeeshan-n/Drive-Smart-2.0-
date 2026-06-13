using Drive_Smart_2._0.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Drive_Smart_2._0.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string projectRoot = Directory.GetCurrentDirectory();

            string dbFolder = Path.Combine(
                projectRoot,
                "Views",
                "Auth",
                "Database");

            Directory.CreateDirectory(dbFolder);

            string dbPath = Path.Combine(
                dbFolder,
                "EmployeeDB.db");

            options.UseSqlite($"Data Source={dbPath}");
        }
    }
}