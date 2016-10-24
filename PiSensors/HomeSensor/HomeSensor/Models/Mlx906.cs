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
        public double AmbiTemp { get; set; }
        public double SkyTemp { get; set; }

        public Mlx906()
        {
            string reading = GetMlx906();
            if(!string.IsNullOrEmpty(reading))
            {
                this.Id = Guid.NewGuid();
                this.Sensor = "pi_sensor_1";
				this.Ip = "MLX906";           
				this.CreatedAt = DateTimeOffset.Now;
                this.AmbiTemp = Convert.ToDouble(reading.Split(',')[0]);
                this.SkyTemp = Convert.ToDouble(reading.Split(',')[1]);
				this.Ok = string.IsNullOrEmpty(reading) ? false : true;
            }
        }
		
		public string GetMlx906()
        {
            string line = "";
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
				start.FileName = "sh";
				start.Arguments = string.Format("-c \"sudo {0}\"", "/home/pi/PiSensors/PiSensors/src/eye2c");// "/home/pi/src2/eye2c";
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                
                using (Process process = Process.Start(start))
                {
					line = process.StandardOutput.ReadToEnd();                  
                }               
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetMlx906 error:  " + ex.Message);
				Common.Logger(ex.Message + ". time: " + DateTime.Today.ToLongDateString() );
            }
            return line;
        }
    }
}
