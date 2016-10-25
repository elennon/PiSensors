using System;
using System.Diagnostics;
using System.IO;

namespace HomeSensor.Models
{
    public class Hflux : Reading
    {
        public double Val { get; set; }
		private bool _ok = false;

        public Hflux()
        {
            this.Id = Guid.NewGuid();
            this.Sensor = Globals.SerialNumber;
            this.Ip = "hflux";
            this.CreatedAt = DateTimeOffset.Now;
            this.Ok = _ok;
        }

        public double GetHflux()
        {
            double value = 0;
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = "/usr/bin/python";
                start.Arguments = "/home/pi/PiSensors/PiSensors/python/hflux.py";
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                string line = "";
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {                        
                        while ((line = reader.ReadLine()) != null)
                        {
                            try
                            {
                                value = Convert.ToDouble(line);
                            }
                            catch (Exception ex)
                            {
                                string h = ex.Message;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hflux error:  " + ex.Message);
                Common.Logger(ex.Message);
            }
			_ok = true;
            return value;
        }
    }
}
