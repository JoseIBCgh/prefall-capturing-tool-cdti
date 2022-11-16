using OpenCvSharp;
using System.Collections.Generic;

namespace ibcdatacsharp.UI.Graphs
{
    public class GraphData
    {
        public FrameData[] frames;
        public FrameData this[int index]
        {
            get { return frames[index]; }
        }
        public int length
        {
            get { return frames.Length; }
        }
        public GraphData(List<FrameData> frames)
        {
            this.frames = frames.ToArray();
        }
    }
    public class FrameData
    {
        public double time { get; set; }
        public int frame { get; set; }
        public double accX { get; set; }
        public double accY { get; set; }
        public double accZ { get; set; }

        public double gyrX { get; set; }
        public double gyrY { get; set; }
        public double gyrZ { get; set; }
        public double magX { get; set; }
        public double magY { get; set; }
        public double magZ { get; set; }
        public FrameData(string csvLine)
        {
            string[] values = csvLine.Split(' ');
            time = double.Parse(values[1]);
            frame = int.Parse(values[2]);
            accX = double.Parse(values[3]);
            accY = double.Parse(values[4]);
            accZ = double.Parse(values[5]);
            gyrX = double.Parse(values[6]);
            gyrY = double.Parse(values[7]);
            gyrZ = double.Parse(values[8]);
            magX = double.Parse(values[9]);
            magY = double.Parse(values[10]);
            magZ = double.Parse(values[11]);
        }
    }
}
