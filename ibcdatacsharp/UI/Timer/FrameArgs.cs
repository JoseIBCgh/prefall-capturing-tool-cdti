using System;

namespace ibcdatacsharp.UI.Timer
{
    // Clase que almacena los datos del Graph Window
    public class FrameArgs : EventArgs
    {
        public int frame { get; set; }
        public double elapsed { get; set; }
    }
}
