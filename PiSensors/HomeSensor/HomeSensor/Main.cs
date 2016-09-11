using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RPi.I2C.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using HomeSensor.Models;
using System.ComponentModel;

namespace HomeSensor
{
class Programer
    {        
		private static bool run = true;
		static BackgroundWorker _bw = new BackgroundWorker();
		public static List<double> sdpReadings = new List<double> ();
		private static object lck = new object();

        static void Main(string[] args)
        {     
			_bw.DoWork += bw_DoWork;
			//_bw.R
            while(run)
			{    
				System.Threading.Thread.Sleep(30000);
				try {
                    GetMlx906().Wait();
                    GetCavityTemp().Wait();
                    GetSdp610().Wait();
                    GetSht15().Wait();
                    GetBMP180().Wait();
                } catch (Exception ex) {
					string h = ex.Message;
				}					
            }
		}

		static void bw_DoWork (object sender, DoWorkEventArgs e)
		{
			while (run) { 
				double dbl = ((Sdp610)(e.Argument)).GetSdp610 ();
				sdpReadings.Add (dbl);
				System.Threading.Thread.Sleep(8000);
			}
		}

        private static async Task GetSht15()
        {
            Sht15 sht = new Sht15();
            await Common.PostReading(sht, "Sht15");
        }

        private static async Task GetSdp610()
		{
			Sdp610 sdp = new Sdp610 (); 			
			double _val;
			lock (lck) {
				_val = sdpReadings.Average ();
				sdpReadings.Clear ();
			}
			sdp.val = _val;
			sdp._time = DateTime.Now;
            await Common.PostReading(sdp, "sdp610");
        }

        private static async Task GetMlx906()
        {
            Mlx906 mlx = new Mlx906();
            await Common.PostReading(mlx, "MLX906");
        }

        private static async Task GetCavityTemp()
        {
            CavityTemp ct = new CavityTemp();
            await Common.PostReading(ct, "cavitytemps");
        }

        private static async Task GetBMP180()
		{
			var bmp = new Bmp180 ("/dev/i2c-1");
            await Common.PostReading(bmp, "Bmp180");
		}	
    }
}