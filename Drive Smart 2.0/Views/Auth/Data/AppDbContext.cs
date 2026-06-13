using Drive_Smart_2._0.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Windows;

namespace Drive_Smart_2._0.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string projectRoot =
                Directory.GetParent(AppContext.BaseDirectory)!
                         .Parent!
                         .Parent!
                         .Parent!
                         .FullName;

            string dbPath = Path.Combine(
                projectRoot,
                "Views",
                "Auth",
                "Database",
                "EmployeeDB.db");

            Directory.CreateDirectory(
                Path.GetDirectoryName(dbPath)!);
            MessageBox.Show(dbPath);

            options.UseSqlite($"Data Source={dbPath}");
        }
    }
}