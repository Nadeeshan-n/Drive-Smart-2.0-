using Drive_Smart_2._0.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Windows;

namespace Drive_Smart_2._0.Data
{
    //used abstraction on here
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
            //MessageBox.Show(dbPath);

            options.UseSqlite($"Data Source={dbPath}");
        }
    }
}

/*Explanation:
 * This class is responsible for connecting the Drive Smart 2.0 application to the SQLite database using Entity Framework Core.
 * acts as a bridge between the application and the database.
 * Microsoft.EntityFrameworkCore --> Used to perform database operations without writing SQL manually.
 * DbContext is the main class in Entity Framework Core that:
 *  Connects to the database
    Creates tables
    Reads data
    Inserts data
    Updates data
    Deletes data
 * The DbSet<Employee> property represents the Employees table. It allows us to perform CRUD operations on employee records using C# objects instead of SQL queries.
 * This code obtains the root directory of the project instead of the build directory. This ensures that the database file is stored in a fixed location and is not recreated every time the application is rebuilt.
 */