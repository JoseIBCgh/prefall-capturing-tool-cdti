using ibcdatacsharp.UI.Timer;
using System;

namespace ibcdatacsharp.UI.Device
{
    public class DeviceArgs : EventArgs
    {
        public FrameArgs frameArgs { get; set; }
        public RawArgs rawArgs { get; set; }
        public AngleArgs angleArgs { get; set; }
    }
}
