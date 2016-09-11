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

		private static HttpClient client = new HttpClient();

		public async Task PostReading(Sdp610 rd)
		{
			client = new HttpClient();
			try
			{
				string resourceAddress = "http://192.168.43.49/moosareback/api/sdp610";
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

		public double GetSdp610()
		{
			try {				
				ProcessStartInfo start = new ProcessStartInfo();
				start.FileName = "php"; //"/etc/php5/cli/php.ini";
				start.Arguments = "/home/pi/sensors_php/sdp.php";
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
				//await PostReading(sdp);
			}
			catch (Exception ex)
			{
				Console.WriteLine("error:  " + ex.Message);
				string error2 = ex.Message;
			}
			return val;
		}
	}
}
