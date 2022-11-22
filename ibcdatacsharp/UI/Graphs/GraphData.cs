﻿using OpenCvSharp;
using System.Collections.Generic;
using System.Globalization;

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
        public int minFrame = 0;
        public int maxFrame
        {
            get { return frames[frames.Length - 1].frame; }
        }
        public double minTime
        {
            get { return frames[0].time; }
        }
        public double maxTime
        {
            get { return frames[frames.Length - 1].time; }
        }
        public double time(int frame){
            int index = frame - minFrame;
            return frames[index].time;
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
            time = double.Parse(values[1], CultureInfo.InvariantCulture);
            frame = int.Parse(values[2]);
            accX = double.Parse(values[3], CultureInfo.InvariantCulture);
            accY = double.Parse(values[4], CultureInfo.InvariantCulture);
            accZ = double.Parse(values[5], CultureInfo.InvariantCulture);
            gyrX = double.Parse(values[6], CultureInfo.InvariantCulture);
            gyrY = double.Parse(values[7], CultureInfo.InvariantCulture);
            gyrZ = double.Parse(values[8], CultureInfo.InvariantCulture);
            magX = double.Parse(values[9], CultureInfo.InvariantCulture);
            magY = double.Parse(values[10], CultureInfo.InvariantCulture);
            magZ = double.Parse(values[11], CultureInfo.InvariantCulture);
        }
    }
}
