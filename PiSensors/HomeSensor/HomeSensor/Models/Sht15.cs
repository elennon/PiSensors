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
        public double Temp { get; set; }
        public double Rh { get; set; }
        public double Dew { get; set; }
		private bool _ok = false;

        public Sht15()
        {
            try
            {
			    this.Id = Guid.NewGuid ();
                this.Sensor = "pi_sensor_1";
                this.Ip = "sht15";
				this.CreatedAt = DateTimeOffset.Now;
			    GetSht15 ();
			    this.Ok = _ok;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetSht15 error:  " + ex.Message);
				Common.Logger(ex.Message + ". time: " + DateTime.Today.ToLongDateString() );
            }
        }

        public void GetSht15()
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "/usr/bin/python";
            start.Arguments = "/usr/local/bin/sht -v -trd 4 17";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            string line = "";            
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        switch (line.Split(':')[0])
                        {
                            case "rh":
                                this.Rh = Convert.ToDouble(line.Split(':')[1]);
                                break;
                            case "temperature":
                                this.Temp = Convert.ToDouble(line.Split(':')[1]);
                                break;
                            case "dew_point":
                                this.Dew = Convert.ToDouble(line.Split(':')[1]);
                                break;
                        }
						_ok = true;
                    }
                }
            }
        }
    }
}
