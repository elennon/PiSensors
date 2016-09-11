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

        public Sht15()
        {
            _id = Guid.NewGuid();
            ok = 1;
            msg = "OK";
            sensor = "pi_sensor_1";
            ip = "sht15";
            time = DateTime.Now;
            createdAt = DateTime.Now;
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
                                this.rh = Convert.ToDouble(line.Split(':')[1]);
                                break;
                            case "temperature":
                                this.temp = Convert.ToDouble(line.Split(':')[1]);
                                break;
                            case "dew_point":
                                this.dew = Convert.ToDouble(line.Split(':')[1]);
                                break;
                        }
                    }
                }
            }
        }
    }
}
