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
		public static Sdp610 sdp = new Sdp610 ();
		private static object lck = new object();

        static void Main(string[] args)
        {     
			//_bw.DoWork += bw_DoWork;
			//_bw.R
            while(run)
			{    
				System.Threading.Thread.Sleep(30000);
				try {
					Mlx906 mlx = new Mlx906 (); 
				    mlx.GetMlx906();
					//GetMlx906().Wait();
					//GetCavityTemp().Wait();
					//GetSdp610 ().Wait ();
					//GetSht15 ().Wait();
					//GetBMP180 ().Wait();
				} catch (Exception ex) {
					string h = ex.Message;
				}					
				//PostReading(r).GetAwaiter().GetResult();
                //System.Threading.Thread.Sleep(60000);
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

		private static async Task GetMlx906()
		{
			Mlx906 mlx = new Mlx906 (); 
			await mlx.GetMlx906 ();
		}

		private static async Task GetCavityTemp()
		{
			CavityTemp ct = new CavityTemp (); 
			await ct.GetCavityTemp ();
		}

		private static async Task GetSdp610()
		{
			//Sdp610 sht = new Sdp610 (); 
			sdp = new Sdp610()
			{
				_id = Guid.NewGuid(),
				ok = 1,
				msg = "OK",
				sensor = "pi_sensor_1",
				ip = "sdp610",
				time = DateTime.Now,
				createdAt = DateTime.Now
			};
			double _val;
			lock (lck) {
				_val = sdpReadings.Average ();
				sdpReadings.Clear ();
			}
			sdp.val = _val;
			sdp._time = DateTime.Now;
			await sdp.PostReading (sdp);
		}

		private static async Task GetSht15()
		{
			Sht15 sht = new Sht15 (); 
			await sht.GetSht15 ();
		}

		private static async Task GetBMP180()
		{
			var bmp = new Bmp180 ("/dev/i2c-1");
			await bmp.PostReading(bmp.reading);
		}	
    }
}