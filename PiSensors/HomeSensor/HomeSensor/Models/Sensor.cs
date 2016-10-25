using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeSensor.Models
{
    public class Sensor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public static class Globals
    {
        public static string SerialNumber = Common.GetSerialNumber();
    }
}
