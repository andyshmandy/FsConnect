using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FsConnect.Events
{
    public class SimStateChangedEventArgs : EventArgs
    {
        public bool Running { get; set; }
    }
}
