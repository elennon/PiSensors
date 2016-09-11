using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeSensor.Models
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
}
