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
		public Guid _id { get; set; }
		public DateTime _time { get; set; }
		public double val { get; set; }

        public Sdp610()
        {
            _id = Guid.NewGuid();
            ok = 1;
            msg = "OK";
            sensor = "pi_sensor_1";
            ip = "sdp610";
            time = DateTime.Now;
            createdAt = DateTime.Now;
        }

		public double GetSdp610()
		{
			try {				
				ProcessStartInfo start = new ProcessStartInfo();
				start.FileName = "php"; //"/etc/php5/cli/php.ini";
				start.Arguments = "/home/pi/PiSensors/PiSensors/sensors_php/sdp.php";
				start.UseShellExecute = false;
				start.RedirectStandardOutput = true;
				string line = "";
				double val = 0;
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
								val = Convert.ToDouble (line);
								return val;
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
			return val;
		}
	}
}
