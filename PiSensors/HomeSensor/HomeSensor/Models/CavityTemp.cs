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
	public class cpReading
	{
		[DataMember]
		public int ok { get; set; }
		[DataMember]
		public string msg { get; set; }
		[DataMember]
		public string sensor { get; set; }
		[DataMember]
		public double data { get; set; }
		[DataMember]
		public long time { get; set; }
	}

	public class CavityTemp : Reading
	{
		public double Val { get; set; }

        public CavityTemp()
        {
            this.Id = Guid.NewGuid();
			this.Sensor = "pi_sensor_1";
			this.Ip = "cavity_temp";           
            GetCavityTemp();
			this.Ok = true;
        }

		public void GetCavityTemp()
		{
			try {				
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
						while ((line = reader.ReadLine ()) != null) 
						{							
							cpReading rd = JsonConvert.DeserializeObject<cpReading>(line);
							this.CreatedAt = new DateTime (rd.time * 1000);
							this.Val = rd.data;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("cavity temp error:  " + ex.Message);
				string error2 = ex.Message;
			}
		}
	}
}
