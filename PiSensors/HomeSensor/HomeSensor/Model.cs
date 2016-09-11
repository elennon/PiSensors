using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeSensor
{
	public class Reading
	{        
		public int ok { get; set; }
		public string msg { get; set; }
		public string sensor { get; set; }
		public string ip { get; set; }
		public DateTime time { get; set; }
		public DateTime createdAt { get; set; }
	}

	public class Bmp180 : Reading
	{
		[Key]
		public Guid _id { get; set; }
		public double pressure { get; set; }
		public double altitude { get; set; }
		public double temp { get; set; }
	}

	public class Sht15 : Reading
	{
		[Key]
		public Guid _id { get; set; }
		public double temp { get; set; }
		public double rh { get; set; }
		public double dew { get; set; }
	}
}
