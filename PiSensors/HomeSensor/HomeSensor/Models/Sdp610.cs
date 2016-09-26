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

namespace HomeSensor.Models
{
	public class Sdp610 : Reading
	{
		public double Val { get; set; }

        public Sdp610()
        {
			this.Id = Guid.NewGuid ();
            this.Sensor = "pi_sensor_1";
            this.Ip = "sdp610";
            this.CreatedAt = DateTime.Now;		
			this.Ok = true;
        }

		public double GetSdp610()
		{
			double value = 0;
			try {				
				ProcessStartInfo start = new ProcessStartInfo();
				start.FileName = "php"; 
				start.Arguments = "/home/pi/PiSensors/PiSensors/sensors_php/sdp.php";
				start.UseShellExecute = false;
				start.RedirectStandardOutput = true;
				string line = "";
				using (Process process = Process.Start(start))
				{
					using (StreamReader reader = process.StandardOutput) 
					{
						bool first = true;
						while ((line = reader.ReadLine ()) != null) 
						{
							if (first) {
								try {
									//long ft = Convert.ToInt64 (line);
									//sdp._time = new DateTime (ft * 1000);
								} catch (Exception ex) {
									string h = ex.Message;
								}
							}							
							else {
								value = Convert.ToDouble (line);
							}
							first = false;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Sdp610 error:  " + ex.Message);
				string error2 = ex.Message;
			}
			return value;
		}
	}
}
