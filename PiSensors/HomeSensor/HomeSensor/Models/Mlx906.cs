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
        public Guid _id { get; set; }
        public DateTime _time { get; set; }
        public double Tambi { get; set; }
        public double Tobj { get; set; }
		[DllImport("magso.so", EntryPoint="main")]
		static extern void frog (ref double ambi, ref double sky);

		public void GetMlx9062()
		{
			string kr = "";
			double r = 0, sky = 0;
			try {
				frog(ref r, ref sky);
				string hu = r.ToString();
			} catch (Exception ex) {
				kr = ex.Message;
			}

		}
		       
		public async Task GetMlx906()
        {
            double ambiTemp = 0, sky = 0;
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
				start.FileName = "sh"; //"/etc/php5/cli/php.ini";
				start.Arguments = string.Format("-c \"sudo {0}\"", "/home/pi/src2/eye2c");// "/home/pi/src2/eye2c";
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                string line = "";
                
                using (Process process = Process.Start(start))
                {
					var scalp = process.StandardOutput.ReadToEnd();
                    using (StreamReader reader = process.StandardOutput)
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            try
                            {
                                var ft = line;
                            }
                            catch (Exception ex)
                            {
                                string h = ex.Message;
                            }
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
        }

    }
}
