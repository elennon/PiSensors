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
using System.Runtime.InteropServices;

namespace HomeSensor.Models
{
    public class Mlx906 : Reading
    {
        public double ambiTemp { get; set; }
        public double skyTemp { get; set; }

        public Mlx906()
        {
            string reading = GetMlx906();
            if(!string.IsNullOrEmpty(reading))
            {
                Id = Guid.NewGuid();
                ok = 1;
                msg = "OK";
                sensor = "pi_sensor_MLX906";
                ip = "906";
                time = DateTime.Now;
                this.ambiTemp = Convert.ToDouble(reading.Split(',')[0]);
                this.skyTemp = Convert.ToDouble(reading.Split(',')[1]);
            }
        }
		
		public string GetMlx906()
        {
            string line = "";
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
				start.FileName = "sh"; //"/etc/php5/cli/php.ini";
				start.Arguments = string.Format("-c \"sudo {0}\"", "/home/pi/src2/eye2c");// "/home/pi/src2/eye2c";
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                
                using (Process process = Process.Start(start))
                {
					var scalp = process.StandardOutput.ReadToEnd();
                    using (StreamReader reader = process.StandardOutput)
                    {
                        line = reader.ReadToEnd();
                    }
                }               
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:  " + ex.Message);
                string error2 = ex.Message;
            }
            return line;
        }

    }
}
