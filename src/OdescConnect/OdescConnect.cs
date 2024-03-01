using System;
using System.IO.Ports;
using System.Linq;
using System.Management;

using FsConnect;
using FsConnect.Events;

namespace OdescConnect
{
    public class OdescConnect
    {
        public event EventHandler<FsDataReceivedEventArgs> FsDataReceived;

        private SerialPort serialPort;


        public OdescConnect()
        {
            serialPort = new SerialPort("COM7", 19200, Parity.None, 8, StopBits.One);

            InitializeConnection();
        }

        private void InitializeConnection()
        {
            serialPort.Open();
        }

        public void HandleReceivedFsData(object sender, FsDataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Count == 0) return;

            if (e.RequestId == (uint)Requests.PlaneInfoRequest)
            {
                var response = (PlaneInfoResponse)e.Data.FirstOrDefault();

                this.SendMotorControlInfo(response.IndicatedAirspeed);
            }
        }

        public double GetElevatorAxisPosition()
        {
            serialPort.WriteLine("f 0");
            string response = serialPort.ReadLine().Trim();

            string position = response.Split(' ').First();

            double elevatorPosition;
            double.TryParse(position, out elevatorPosition);

            return elevatorPosition;
        }

        public void SendMotorControlInfo(double position)
        {
            Console.WriteLine($"Going to position {position}");

            //string motor = "0";
            //string speed = "4";
            //string torque = "7";
            //this.serialPort.WriteLine($"p {motor} {position} {speed} {torque}");

            serialPort.WriteLine($"q 0 0 0 {position}");
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
