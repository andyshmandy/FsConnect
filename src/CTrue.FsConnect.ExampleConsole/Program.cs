using System;
using System.Linq;
using FsConnect.Events;

namespace FsConnect.ExampleConsole
{
    public class FsConnectTestConsole
    {
        public double airSpeed;
        public FsConnectTestConsole()
        {
            airSpeed = 0;
        }

        public static void Main(string[] args)
        {
            string hostName = "localhost";
            uint port = 500;

            // Also supports "somehostname 1234"
            if (args.Length == 2)
            {
                hostName = args[0];
                port = uint.Parse(args[1]);
            }

            var fsConnect = new FsConnect();
            var odConnect = new OdescConnect.OdescConnect();

            // Specify where the SimConnect.cfg should be written to
            fsConnect.SimConnectFileLocation = SimConnectFileLocation.Local;

            // Creates a SimConnect.cfg and connect to Flight Simulator using this configuration.
            fsConnect.Connect("TestApp", hostName, port, SimConnectProtocol.Ipv4);

            fsConnect.FsDataReceived += HandleReceivedFsData;
            fsConnect.FsDataReceived += odConnect.HandleReceivedFsData;

            int planeInfoDefinitionId = fsConnect.RegisterDataDefinition<PlaneInfoResponse>();
            int elevatorDefId = fsConnect.RegisterDataDefinition<ElevatorSet>();
            
            ConsoleKeyInfo cki;

            do
            {
                fsConnect.RequestData((int)Requests.PlaneInfoRequest, planeInfoDefinitionId); ;

                double elevatorPosition = odConnect.GetElevatorAxisPosition();
                fsConnect.UpdateData(elevatorDefId, elevatorPosition * -100000);

                cki = Console.ReadKey();
            } while (cki.Key != ConsoleKey.Escape || true);

            fsConnect.Disconnect();
        }

        private static void HandleReceivedFsData(object sender, FsDataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Count == 0) return;

            if (e.RequestId == (uint)Requests.PlaneInfoRequest)
            {
                PlaneInfoResponse r = (PlaneInfoResponse)e.Data.FirstOrDefault();
                Console.WriteLine($"IndicatedAirspeed: {r.IndicatedAirspeed} ElevatorDeflection: {r.ElevatorDeflection} ElevatorPosition: {r.ElevatorPosition}");
            }
        }
    }
}