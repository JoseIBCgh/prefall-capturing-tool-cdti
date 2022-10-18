using System;

namespace ibcdatacsharp.UI.FileSaver
{
    public class SaveArgs: EventArgs
    {
        public string directory { get; set; }
        public bool csv { get; set; }
        public bool video { get; set; }
    }
}
