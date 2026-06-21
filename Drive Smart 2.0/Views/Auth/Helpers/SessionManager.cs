using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drive_Smart_2._0.Models;

namespace Drive_Smart_2._0.Views.Auth.Helpers
{
    // static class because we want to access the current employee from anywhere in the application without creating an instance of the class
    public static class SessionManager
    {
        // Property to hold the currently logged-in employee
        public static Employee? CurrentEmployee { get; set; }

        // Property to check if an employee is currently logged in
        public static bool IsLoggedIn =>
            CurrentEmployee != null;

        public static void Logout()
        {
            CurrentEmployee = null;
        }
    }
}
