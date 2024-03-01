using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;

namespace FFB_SimConnect
{
    internal class OdescConnector
    {
        private SerialPort serialPort;


        public OdescConnector()
        {
            this.serialPort = new SerialPort("COM7", 19200, Parity.None, 8, StopBits.One);

            this.InitializeConnection();
        }

        private void GetSerialPort()
        {
            var ports = SerialPort.GetPortNames();
            GetOdrivePort();

            foreach (var port in ports)
            {
                Console.WriteLine(port);
            }
        }

        public void InitializeConnection()
        {
            this.serialPort.Open();
        }

        public double GetElevatorAxisPosition()
        {
            this.serialPort.WriteLine("f 0");
            string response = this.serialPort.ReadLine().Trim();

            string position = response.Split(' ').First();

            double elevatorPosition;
            double.TryParse(position, out elevatorPosition);

            return elevatorPosition;
        }

        public void SendPositionData(double position)
        {
            Console.WriteLine($"Going to position {position}");

            //string motor = "0";
            //string speed = "4";
            //string torque = "7";
            //this.serialPort.WriteLine($"p {motor} {position} {speed} {torque}");

            this.serialPort.WriteLine($"q 0 0 0 {position}");
        }

        public static void GetOdrivePort()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                var portnames = SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                var portList = portnames.Select(n => n + " - " + ports.FirstOrDefault(s => s.Contains(n))).ToList();

                foreach (string s in portList)
                {
                    Console.WriteLine(s);
                }
            }
        }

    }
}
