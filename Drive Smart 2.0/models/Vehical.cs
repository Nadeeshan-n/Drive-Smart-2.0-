using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drive_Smart_2._0.models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Brand { get; set; } = "";
        public string Model { get; set; } = "";
        public string PlateNumber { get; set; } = "";
        public int Year { get; set; }
        public string Color { get; set; } = "";
        public double DailyRate { get; set; }
        public string Status { get; set; } = "";
    }
}
