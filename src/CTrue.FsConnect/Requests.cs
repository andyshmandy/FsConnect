using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FsConnect
{
    public enum Requests
    {
        PlaneInfoRequest = 0
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct PlaneInfoResponse
    {
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
}
