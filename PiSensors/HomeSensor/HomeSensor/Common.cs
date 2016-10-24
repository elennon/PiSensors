using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Cache;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using HomeSensor.Models;
using System.Linq;
using System.Diagnostics;

namespace HomeSensor
{
	public static class Common
	{
        private static List<NotSenters> notSenters = new List<NotSenters>();
		private static HttpClient client = new HttpClient();
		public static async Task PostReading(object rd, string url)
		{
			client = new HttpClient();
			try
			{
                string resourceAddress = "http://139.59.172.240:3000/api/" + url;   // "http://192.168.43.167/moosareback/api/" + url;
                string postBody = Common.JsonSerializer(rd);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {            
                    var response = await client.PostAsync(resourceAddress, new StringContent(postBody, Encoding.UTF8, "application/json"));
                    Console.WriteLine("response:  " + response + GetNistTime().ToLongTimeString());
                    if(notSenters.Count > 0)
                    {
                        foreach (var item in notSenters)
                        {
                            await client.PostAsync(item.Url, new StringContent(item.Body, Encoding.UTF8, "application/json"));
                        }
                        notSenters.Clear();
                    }
                }
                else
                {
                    notSenters.Add(new NotSenters { Body = postBody, Url = resourceAddress });
                }
			}
			catch (Exception ex)
			{
				Console.WriteLine("Post error:  " + ex.Message);
				Common.Logger(ex.Message + ". time: " + DateTime.Today.ToLongDateString() );
			}
		}

        public static DateTime GetNistTime()
        {
            DateTime dateTime = DateTime.MinValue;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://nist.time.gov/actualtime.cgi?lzbc=siqm9b");
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore); //No caching
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader stream = new StreamReader(response.GetResponseStream());
                string html = stream.ReadToEnd();//<timestamp time=\"1395772696469995\" delay=\"1395772696469995\"/>
                string time = Regex.Match(html, @"(?<=\btime="")[^""]*").Value;
                double milliseconds = Convert.ToInt64(time) / 1000.0;
                dateTime = new DateTime(1970, 1, 1).AddMilliseconds(milliseconds).ToLocalTime();
            }

            return dateTime;
        }

        public static void Logger(string message)
        {
            File.AppendAllText(@"/home/pi/sensors_log.txt", message + Environment.NewLine);
        }

        public static string JsonSerializer(object objectToSerialize)
		{
			if (objectToSerialize == null)
			{
				throw new ArgumentException("objectToSerialize must not be null");
			}
			MemoryStream ms = null;
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(objectToSerialize.GetType());
			ms = new MemoryStream();
			serializer.WriteObject(ms, objectToSerialize);
			ms.Seek(0, SeekOrigin.Begin);
			StreamReader sr = new StreamReader(ms);
			return sr.ReadToEnd();
		}

		public static T deserializeJSON<T>(string json)
		{
			var instance = typeof(T);
			using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
			{
				DataContractJsonSerializer deserializer = new DataContractJsonSerializer(instance.GetType());
				return (T)deserializer.ReadObject(ms);
			}
		}
			
		public static string GetSerialNumber()
		{
			string line = "", result = "";
			try
			{
				ProcessStartInfo start = new ProcessStartInfo();
				start.FileName = "/bin/bash";
				//start.Arguments = "cat /proc/cpuinfo | grep Serial";
				start.Arguments = string.Format("-c \"sudo {0}\"", "cat /proc/cpuinfo | grep Serial");
				start.UseShellExecute = false;
				start.RedirectStandardOutput = true;
				using (Process process = Process.Start(start))
				{
					using (StreamReader reader = process.StandardOutput)
					{
						while ((line = reader.ReadLine ()) != null) 
						{
							result = line;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("get serial number error:  " + ex.Message);
				Common.Logger(ex.Message + ". time: " + DateTime.Today.ToLongDateString());
			}
			return result;
		}
	}

}

