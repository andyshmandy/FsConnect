using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FsConnect.Events
{
    public class PauseStateChangedEventArgs : EventArgs
    {
        public bool Paused { get; set; }
    }
}
