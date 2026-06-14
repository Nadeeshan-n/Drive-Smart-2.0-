using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drive_Smart_2._0.Models;

namespace Drive_Smart_2._0.Views.Auth
{
    public static class SessionManager
    {
        public static Employee? CurrentEmployee { get; set; }

        public static bool IsLoggedIn =>
            CurrentEmployee != null;

        public static void Logout()
        {
            CurrentEmployee = null;
        }
    }
}
