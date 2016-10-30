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
using System.Web;

namespace HomeSensor
{
class Programer
    {        
		private static bool run = true;
		static BackgroundWorker _bw = new BackgroundWorker();
		public static List<double> sdpReadings = new List<double> ();
		private static object threadLock = new object();
		private static Sdp610 sdp = new Sdp610 ();
        
        static void Main(string[] args)
        {
			Common.counter = 0;
			Common.notSenters = new List<NotSenters> ();
            Common.CheckSensor().Wait();
            
			//_bw.DoWork += bw_DoWork;
			//_bw.RunWorkerAsync (sdp);
            while(run)
			{    
				System.Threading.Thread.Sleep(5);
				try {
                    //GetMlx906().Wait();
                    //GetCavityTemp().Wait();
                    //GetSdp610().Wait();
                    //GetSht15().Wait();
                    //GetBMP180().Wait();
					GetHflux().Wait();
                } catch (Exception ex) {
					Common.Logger(ex.Message + ". time: " + DateTime.Today.ToLongDateString() );
				}					
            }
			Common.Logger ("Exited while loop...");
		}

		static void bw_DoWork (object sender, DoWorkEventArgs e)
		{
			while (run) { 
				double dbl = ((Sdp610)(e.Argument)).GetSdp610 ();
				sdpReadings.Add (dbl);
				System.Threading.Thread.Sleep(1000);
			}
		}

        private static async Task GetHflux()
        {
            Hflux hflx = new Hflux();
			Console.WriteLine ("flux: " + hflx.Val);

			if (CheckInternet ()) { 
				if (Common.notSenters.Count () > 0) {
					Common.SendNotSent();
				}
				await Common.PostReading (hflx, "Hflux");
			} else {
				Common.AddToNotSent (hflx, "Hflux");
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
			lock (threadLock) {
				_val = sdpReadings.Average ();
				sdpReadings.Clear ();
			}
			sdp.Val = _val;
			sdp.Ok = _val != null ? true : false;
			sdp.CreatedAt = DateTime.Now;
            await Common.PostReading(sdp, "sdp610");
        }

        private static async Task GetMlx906()
        {
            Mlx906 mlx = new Mlx906();
            await Common.PostReading(mlx, "MLX906");
			Console.WriteLine(mlx.AmbiTemp.ToString() + " : " + mlx.SkyTemp.ToString());
        }

        private static async Task GetCavityTemp()
        {
            var cts = cpReading.GetCavityTemp();
            foreach (var item in cts)
            {
                await Common.PostReading(item, "cavityTemp");
            }
        }

        private static async Task GetBMP180()
		{
			var bmp = new Bmp180 ("/dev/i2c-1");
            await Common.PostReading(bmp, "Bmp180");
		}

		private static bool CheckInternet ()
		{
			try
			{
				using (var client = new System.Net.WebClient())
				{
					using (var stream = client.OpenRead("http://www.google.com"))
					{
						return true;
					}
				}
			}
			catch
			{
				return false;
			}
		}
    }
}