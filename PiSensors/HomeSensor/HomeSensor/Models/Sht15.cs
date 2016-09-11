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
    public class Sht15 : Reading
    {
        public Guid _id { get; set; }
        public double temp { get; set; }
        public double rh { get; set; }
        public double dew { get; set; }

        private static HttpClient client = new HttpClient();

        private async Task PostReading(Sht15 rd)
        {
            client = new HttpClient();
            try
            {
                string resourceAddress = "http://192.168.43.49/moosareback/api/Sht15";
                //var gg = await client.GetStringAsync(resourceAddress);
                //Console.WriteLine("plop:  " + gg);
                string postBody = Common.JsonSerializer(rd);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                await client.PostAsync(resourceAddress, new StringContent(postBody, Encoding.UTF8, "application/json"));
            }
            catch (HttpRequestException hre)
            {
                Console.WriteLine("error:  " + hre.Message);
                string error = hre.Message;
            }

            catch (Exception ex)
            {
                Console.WriteLine("error:  " + ex.Message);
                string error2 = ex.Message;
            }
        }

        public async Task GetSht15()
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "/usr/bin/python";
            start.Arguments = "/usr/local/bin/sht -v -trd 4 17";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            string line = "";
            Sht15 sht = new Sht15()
            {
				_id = Guid.NewGuid(),
                ok = 1,
                msg = "OK",
                sensor = "pi_sensor_1",
                ip = "sht15",
                time = DateTime.Now,
                createdAt = DateTime.Now
            };

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        switch (line.Split(':')[0])
                        {
                            case "rh":
                                sht.rh = Convert.ToDouble(line.Split(':')[1]);
                                break;
                            case "temperature":
                                sht.temp = Convert.ToDouble(line.Split(':')[1]);
                                break;
                            case "dew_point":
                                sht.dew = Convert.ToDouble(line.Split(':')[1]);
                                break;
                        }
                        Console.Write(line);
                    }
                }
            }
			await PostReading(sht);
        }
    }
}
