using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

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
        public int minFrame
        {
            get { return frames[0].frame; }
        }
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
            try
            {
                int index = frame - minFrame;
                return frames[index].time;
            }
            catch(Exception e)
            {
                Trace.WriteLine(minFrame);
                throw e;
            }
        }
        public GraphData(FrameData[] frames)
        {
            this.frames = frames;
        }
        public int numIMUs
        {
            get
            {
                return frames[0].numIMUs();
            }
        }
    }
    public abstract class FrameData
    {
        public double time { get; set; }
        public int frame { get; set; }
        protected double parseDouble(string s)
        {
            string s_point = s.Replace(",", ".");
            double result = double.Parse(s_point, CultureInfo.InvariantCulture);
            return result;
        }
        protected float parseFloat(string s)
        {
            string s_point = s.Replace(",", ".");
            float result = float.Parse(s_point, CultureInfo.InvariantCulture);
            return result;
        }
        public abstract int numIMUs();
    }
    public class FrameData1IMU: FrameData
    {
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
        public FrameData1IMU(string csvLine)
        {
            string[] values = csvLine.Split(' ');
            if(values.Length != 15 && values.Length != 12)
            {
                throw new Exception("Deben haber 12 o 15 valores por fila");
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
            if (values.Length == 14)
            {
                laccX = parseDouble(values[12]);
                laccY = parseDouble(values[13]);
                laccZ = parseDouble(values[14]);
            }
            else
            {
                laccX = 0;
                laccY = 0;
                laccZ = 0;
            }
        }

        public override int numIMUs()
        {
            return 1;
        }
    }
    public class FrameData2IMUs : FrameData
    {
        public double angleX { get; set; }
        public double angleY { get; set; }
        public double angleZ { get; set; }
        public Vector3 angularVelocity { get; set; }
        public Vector3 angularAcceleration { get; set; }
        public FrameData2IMUs(string csvLine)
        {
            string[] values = csvLine.Split(' ');
            if (values.Length != 12) //default 12
            {
                throw new Exception("Deben haber 12 valores por fila");
            }
            time = parseDouble(values[1]);
            frame = int.Parse(values[2]);
            angleX = parseDouble(values[3]);
            angleY = parseDouble(values[4]);
            angleZ = parseDouble(values[5]);
            angularVelocity = new Vector3(parseFloat(values[6]), parseFloat(values[7]),
                parseFloat(values[8]));
            angularAcceleration = new Vector3(parseFloat(values[9]), parseFloat(values[10]),
                parseFloat(values[11]));
        }
        public override int numIMUs()
        {
            return 2;
        }
    }
}
