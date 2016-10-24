using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace HomeSensor.Models
{
	[DataContract]
	public static class cpReading
	{
		[DataMember]
		public static int ok { get; set; }
		[DataMember]
		public static string msg { get; set; }
		[DataMember]
		public static string sensor { get; set; }
		[DataMember]
		public static double data { get; set; }
		[DataMember]
		public static long time { get; set; }


        public static List<CavityTemp> GetCavityTemp()
        {
            List<CavityTemp> lst = new List<CavityTemp>();
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = "php";
                start.Arguments = "/home/pi/PiSensors/PiSensors/sensors_php/cavityTemp.php";
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                string line = "";

                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
							Common.Logger(ex.Message + ". time: " + DateTime.Today.ToLongDateString() );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("cavity temp error:  " + ex.Message);
                Common.Logger(ex.Message);
            }
            return lst;
        }
    }

	public class CavityTemp : Reading
	{
		public double Val { get; set; }

        public CavityTemp()
        {
            this.Id = Guid.NewGuid();
			//this.Sensor = "pi_sensor_1";
			this.Ip = "cavity_temp";           
			this.CreatedAt = DateTimeOffset.Now;
            this.Ok = true;
        }

	}
}
