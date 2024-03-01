using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using FFB_SimConnect;

using FsConnect;
using FsConnect.Events;

using Microsoft.FlightSimulator.SimConnect;

namespace FsConnect.ExampleConsole
{
    public enum Requests
    {
        PlaneInfoRequest = 0
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PlaneInfoResponse
    {
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        //public String Title;

        //[SimVar(UnitId = FsUnit.Radians)]
        //public UInt16 AileronAverageDeflection;

        //[SimVar(UnitId = FsUnit.Position16k)]
        //public double AileronPosition;

        [SimVar(UnitId = FsUnit.Radians)]
        public double ElevatorDeflection;

        [SimVar(UnitId = FsUnit.Position16k)]
        public double ElevatorPosition;

        [SimVar(UnitId = FsUnit.Knots)]
        public double IndicatedAirspeed;

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ElevatorSet
    {
        [SimVar(UnitId = FsUnit.Position16k)]
        public double ElevatorPosition;
    }

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

            FsConnect fsConnect = new FsConnect();

            // Specify where the SimConnect.cfg should be written to
            fsConnect.SimConnectFileLocation = SimConnectFileLocation.Local;

            // Creates a SimConnect.cfg and connect to Flight Simulator using this configuration.
            fsConnect.Connect("TestApp", hostName, port, SimConnectProtocol.Ipv4);

            // Other alternatives, use existing SimConfig.cfg and specify config index:
            // fsConnect.Connect(1);
            // or
            // fsConnect.Connect();

            fsConnect.FsDataReceived += HandleReceivedFsData;

            int planeInfoDefinitionId = fsConnect.RegisterDataDefinition<PlaneInfoResponse>();
            int elevatorDefId = fsConnect.RegisterDataDefinition<ElevatorSet>();

            //fsConnect.UpdateData(elevatorDefId, elevPosition, 0);
            var odConnector = new OdescConnector();
            ConsoleKeyInfo cki;

            while (true)
            {
                fsConnect.RequestData((int)Requests.PlaneInfoRequest, planeInfoDefinitionId); ;

                odConnector.SendPositionData(.1);
                double elevatorPosition = odConnector.GetElevatorAxisPosition();
                fsConnect.UpdateData(elevatorDefId, elevatorPosition * -100000);
                Thread.Sleep(10);
                //cki = Console.ReadKey();
            } //while (cki.Key != ConsoleKey.Escape || true);

            fsConnect.Disconnect();
        }

        private static void HandleReceivedFsData(object sender, FsDataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data.Count == 0) return;

            if (e.RequestId == (uint)Requests.PlaneInfoRequest)
            {


                PlaneInfoResponse r = (PlaneInfoResponse)e.Data.FirstOrDefault();
                Console.WriteLine($"IndicatedAirspeed: {r.IndicatedAirspeed} ElevatorDeflection: {r.ElevatorDeflection} ElevatorPosition: {r.ElevatorPosition}");
                //Console.WriteLine($"AileronPosition: {r.AileronPosition}  ElevatorPosition: {r.ElevatorPosition}");
            }
        }
    }
}