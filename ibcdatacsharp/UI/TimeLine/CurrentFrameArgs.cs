namespace ibcdatacsharp.UI.TimeLine
{
    // Datos que se usan en el evento cuando la currentFrameLine se mueve manualmente
    public class CurrentFrameArgs
    {
        public bool start { get; set; }
        public bool end { get { return !start; } set { start = !value; } }
    }
}
