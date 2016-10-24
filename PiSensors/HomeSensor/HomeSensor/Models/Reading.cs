using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeSensor.Models
{
	public class Reading
    {
        public Guid Id { get; set; }
        public bool Ok { get; set; }
        public string Message { get; set; }
        public string Sensor { get; set; }
        public string Ip { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
