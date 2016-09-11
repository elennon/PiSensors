using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPi.I2C.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HomeSensor.Models
{
    public class Bmp180 : Reading, IDisposable
    {
        public Guid _id { get; set; }
        public double pressure { get; set; }
        public double altitude { get; set; }
        public double temp { get; set; }

        private I2CBus bus;

        private short AC1, AC2, AC3, B1, B2, MB, MC, MD;
        private ushort AC4, AC5, AC6;

        private int deviceID = 0x77;
        private static HttpClient client = new HttpClient();
        public Bmp180 reading;

		public Bmp180 ()
		{
		}

        public Bmp180(string _busName)
        {
            bus = RPi.I2C.Net.I2CBus.Open(_busName);
            if (!Detect())
            {
                Console.WriteLine("Some error occurred during communication with the sensor. Please check the sensor.");
                Console.ReadLine();
                Environment.Exit(0);
            }
            InitParams();
            reading = new Bmp180()
            {
                _id = Guid.NewGuid(),
                ok = 1,
                msg = "OK",
                sensor = "pi_sensor_1",
                ip = "666",
                time = DateTime.Now,
                createdAt = DateTime.Now,
                pressure = this.GetPressure(),
                temp = this.GetTemperature()
            };
        }

        public async Task PostReading(Bmp180 rd)
        {
            client = new HttpClient();
            try
            {
                string resourceAddress = "http://192.168.43.49/moosareback/api/Bmp180";
                //var gg = await client.GetStringAsync(resourceAddress);
                //Console.WriteLine("plop:  " + gg);
                string postBody = Common.JsonSerializer(rd);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                await client.PostAsync(resourceAddress, new StringContent(postBody, Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:  " + ex.Message);
                string error2 = ex.Message;
            }
        }

        public double GetTemperature()
        {
            byte[] bytes = new byte[] { 0xF4, 0x2E };
            bus.WriteBytes(deviceID, bytes);
            System.Threading.Thread.Sleep(6);
            long ut = ReadUnsignedWord(0xF6, 0xF7);
            long x1 = (long)((ut - AC6) * AC5 / Math.Pow(2, 15));
            long x2 = (long)(MC * Math.Pow(2, 11) / (x1 + MD));
            long b5 = x1 + x2;
            long temp = (long)((b5 + 8) / Math.Pow(2, 4));
            return temp * 0.1;

        }

        public double GetPressure()
        {
            double current_temperature = GetTemperature();
            byte[] bytes = new byte[] { 0xF4, (byte)(0x34 + (3 << 6)) };
            bus.WriteBytes(deviceID, bytes);
            System.Threading.Thread.Sleep(28);
            bus.WriteByte(deviceID, (byte)0xF6);
            byte ms = bus.ReadBytes(deviceID, 1)[0];
            bus.WriteByte(deviceID, (byte)0xF7);
            byte ls = bus.ReadBytes(deviceID, 1)[0];
            bus.WriteByte(deviceID, (byte)0xF8);
            byte xs = bus.ReadBytes(deviceID, 1)[0];
            double pu = (ms * 256.0) + ls + (xs / 256.0);
            double s, x, y, z;
            double x0 = AC1;
            double x1 = 160.0 * Math.Pow(2, -13) * AC2;
            double x2 = Math.Pow(160, 2) * Math.Pow(2, -25) * B2;
            double c3 = 160.0 * Math.Pow(2, -15) * AC3;
            double c4 = Math.Pow(10, -3) * Math.Pow(2, -15) * AC4;
            double b1 = Math.Pow(160, 2) * Math.Pow(2, -30) * B1;
            double y0 = c4 * Math.Pow(2, 15);
            double y1 = c4 * c3;
            double y2 = c4 * b1;
            double p0 = (3791.0 - 8.0) / 1600.0;
            double p1 = 1.0 - 7357.0 * Math.Pow(2, -20);
            double p2 = 3038.0 * 100.0 * Math.Pow(2, -36);
            s = current_temperature - 25.0;
            x = (x2 * Math.Pow(s, 2)) + (x1 * s) + x0;
            y = (y2 * Math.Pow(s, 2)) + (y1 * s) + y0;
            z = (pu - x) / y;
            double P = (p2 * Math.Pow(z, 2)) + (p1 * z) + p0;
            return P;
        }

        public bool Detect()
        {
            byte[] results;
            try
            {
                bus.WriteByte(0x77, (byte)0xD0);
                results = bus.ReadBytes(0x77, 1);
                if (results[0] != 0x55) return false;
                InitParams();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void InitParams()
        {
            AC1 = ReadSignedWord(0xAA, 0xAB);
            AC2 = ReadSignedWord(0xAC, 0xAD);
            AC3 = ReadSignedWord(0xAE, 0xAF);
            AC4 = ReadUnsignedWord(0xB0, 0xB1);
            AC5 = ReadUnsignedWord(0xB2, 0xB3);
            AC6 = ReadUnsignedWord(0xB4, 0xB5);
            B1 = ReadSignedWord(0xB6, 0xB7);
            B2 = ReadSignedWord(0xB8, 0xB9);
            MB = ReadSignedWord(0xBA, 0xBB);
            MC = ReadSignedWord(0xBC, 0xBD);
            MD = ReadSignedWord(0xBE, 0xBF);
        }

        private short ReadSignedWord(int msbAddress, int lsbAddress)
        {
            bus.WriteByte(deviceID, (byte)msbAddress);
            byte[] bytes = bus.ReadBytes(deviceID, 1);
            int raw = bytes[0] << 8;
            bus.WriteByte(deviceID, (byte)lsbAddress);
            bytes = bus.ReadBytes(deviceID, 1);
            raw |= bytes[0];
            return (short)raw;
        }

        private ushort ReadUnsignedWord(int msbAddress, int lsbAddress)
        {
            bus.WriteByte(deviceID, (byte)msbAddress);
            byte[] bytes = bus.ReadBytes(deviceID, 1);
            ushort raw = (ushort)((ushort)bytes[0] << 8);
            bus.WriteByte(deviceID, (byte)lsbAddress);
            bytes = bus.ReadBytes(deviceID, 1);
            raw |= (ushort)bytes[0];
            return raw;
        }

        public void Dispose()
        {
            bus.Dispose();
        }
    }
}
