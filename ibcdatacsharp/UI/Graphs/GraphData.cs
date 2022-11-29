using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public double laccX { get; set; }
        public double laccY { get; set; }
        
        public double laccZ { get; set; }
        public FrameData(string csvLine)
        {
            string[] values = csvLine.Split(' ');
            if(values.Length > 12)
            {
                throw new Exception("Deben haber 12 valores por fila");
            }
            time = parseDouble(values[1]);
            frame = int.Parse(values[2]);
            accX = parseDouble(values[3]);
            accY = parseDouble(values[4]);
            accZ = parseDouble(values[5]);
            gyrX = parseDouble(values[6]);
            gyrY = parseDouble(values[7]);
            gyrZ = parseDouble(values[8]);
            magX = parseDouble(values[9]);
            magY = parseDouble(values[10]);
            magZ = parseDouble(values[11]);
            laccX = parseDouble(values[12]);
            laccY = parseDouble(values[13]);
            laccZ = parseDouble(values[14]);
        }
        private double parseDouble(string s)
        {
            string s_comma = s.Replace(",", ".");
            double result =  double.Parse(s_comma, CultureInfo.InvariantCulture);
            if(Math.Abs(result) > 10000)
            {
                throw new Exception("Conversion a double incorrecta");
            }
            return result;
        }
    }
}
