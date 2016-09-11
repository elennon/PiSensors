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
		public Guid _id { get; set; }
		public DateTime _time { get; set; }
		public double val { get; set; }

		private static HttpClient client = new HttpClient();

		private async Task PostReading(CavityTemp rd)
		{
			client = new HttpClient();
			try
			{
				string resourceAddress = "http://192.168.43.49/moosareback/api/cavitytemps";
				//var gg = await client.GetStringAsync(resourceAddress);
				//Console.WriteLine("plop:  " + gg);
				string postBody = Common.JsonSerializer(rd);
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				var response = await client.PostAsync(resourceAddress, new StringContent(postBody, Encoding.UTF8, "application/json"));
				//Console.WriteLine("response:  " + response);
			}
			catch (Exception ex)
			{
				Console.WriteLine("error:  " + ex.Message);
				string error2 = ex.Message;
			}
		}

		public async Task GetCavityTemp()
		{
			try {				
				ProcessStartInfo start = new ProcessStartInfo();
				start.FileName = "php"; //"/etc/php5/cli/php.ini";
				start.Arguments = "/home/pi/sensors_php/cavityTemp.php";
				start.UseShellExecute = false;
				start.RedirectStandardOutput = true;
				string line = "";
				CavityTemp cp = new CavityTemp()
				{
					_id = Guid.NewGuid(),
					ok = 1,
					msg = "OK",
					sensor = "cavity_temp",
					ip = "pi_sensor_1",
					time = DateTime.Now,
					createdAt = DateTime.Now
				};

				using (Process process = Process.Start(start))
				{
					using (StreamReader reader = process.StandardOutput) 
					{
						while ((line = reader.ReadLine ()) != null) 
						{							
							cpReading rd = JsonConvert.DeserializeObject<cpReading>(line);
							cp._time = new DateTime (rd.time * 1000);
							cp.val = rd.data;
						}
					}
				}
				await PostReading(cp);
			}
			catch (Exception ex)
			{
				Console.WriteLine("error:  " + ex.Message);
				string error2 = ex.Message;
			}
		}
	}
}
