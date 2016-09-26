using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HomeSensor
{
	public static class Common
	{
		private static HttpClient client = new HttpClient();
		public static async Task PostReading(object rd, string url)
		{
			client = new HttpClient();
			try
			{
				string resourceAddress = "http://192.168.43.167/moosareback/api/" + url;
				//var gg = await client.GetStringAsync(resourceAddress);
				//Console.WriteLine("tester:  " + gg);
				string postBody = Common.JsonSerializer(rd);
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				var response = await client.PostAsync(resourceAddress, new StringContent(postBody, Encoding.UTF8, "application/json"));
				Console.WriteLine("response:  " + response);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Post error:  " + ex.Message);
				string error2 = ex.Message;
			}
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
	}
}

