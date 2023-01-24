using OpenCvSharp.XImgProc.Segmentation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Navigation;

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
                return frames[0].numIMUs;
            }
        }
    }
    #region Factories
    public class FrameDataMetaFactory
    {
        public List<FrameDataFactory> factories;
        private FrameDataFactory current;
        public FrameDataMetaFactory()
        {
            factories = new List<FrameDataFactory>();
            factories.Add(new FrameDataFactory1IMU());
            factories.Add(new FrameDataFactory2IMUs());
            factories.Add(new FrameDataFactorySagital());
        }
        public void changeHeader(string header)
        {
            int minSimilarity = int.MaxValue;
            foreach(FrameDataFactory factory in factories)
            {
                int similarity = factory.compareSimilarity(header);
                if(similarity < minSimilarity)
                {
                    minSimilarity = similarity;
                    current = factory;
                }
            }
            //current = factories.Aggregate((curMin, x) => curMin == null || (x.similarity ?? int.MaxValue) < x.similarity ? x : curMin);
        }
        public void addLine(string header)
        {
            current.addLine(header);
        }
        public GraphData getData()
        {
            return current.getData();
        }
    }
    public abstract class FrameDataFactory
    {
        protected virtual string header { get; }
        public abstract void addLine(string line);
        public abstract GraphData getData();
        public int compareSimilarity(string header)
        {
            int value1 = 0;
            foreach (char c in this.header)
            {
                int tmp = c;
                value1 += c;
            }
            int value2 = 0;
            foreach (char c in header)
            {
                int tmp = c;
                value2 += c;
            }
            int similarity = Math.Abs(value1 - value2);
            return similarity;
        }
    }
    public class FrameDataFactory1IMU : FrameDataFactory
    {
        protected override string header { get { return Config.csvHeader1IMU; } }
        private List<FrameData1IMU> data;
        public FrameDataFactory1IMU()
        {
            data = new List<FrameData1IMU>();
        }
        public override void addLine(string line)
        {
            data.Add(new FrameData1IMU(line));
        }
        public override GraphData getData()
        {
            return new GraphData(data.ToArray());
        }
    }
    public class FrameDataFactory2IMUs : FrameDataFactory
    {
        protected override string header { get { return Config.csvHeader2IMUs; } }
        private List<FrameData2IMUs> data;
        public FrameDataFactory2IMUs()
        {
            data = new List<FrameData2IMUs>();
        }
        public override void addLine(string line)
        {
            data.Add(new FrameData2IMUs(line));
        }
        public override GraphData getData()
        {
            return new GraphData(data.ToArray());
        }
    }
    public class FrameDataFactorySagital : FrameDataFactory
    {
        protected override string header { get { return Config.csvHeaderSagital; } }
        private List<FrameDataSagital> data;
        public FrameDataFactorySagital()
        {
            data = new List<FrameDataSagital>();
        }
        public override void addLine(string line)
        {
            data.Add(new FrameDataSagital(line));
        }
        public override GraphData getData()
        {
            return new GraphData(data.ToArray());
        }
    }
    #endregion Factories
    public abstract class FrameData
    {
        public virtual int numIMUs { get; }
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
        protected float getFloat(string[] values, int index)
        {
            if(index < values.Length)
            {
                return parseFloat(values[index]);
            }
            else
            {
                return 0f;
            }
        }
        protected double getDouble(string[] values, int index)
        {
            if (index < values.Length)
            {
                return parseDouble(values[index]);
            }
            else
            {
                return 0.0;
            }
        }
        protected int getInt(string[] values, int index)
        {
            if (index < values.Length)
            {
                return int.Parse(values[index]);
            }
            else
            {
                return 0;
            }
        }
    }
    public class FrameData1IMU: FrameData
    {
        public override int numIMUs { get { return 1; } }
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

        public double quatX { get; set; }
        public double quatY { get; set; }
        public double quatZ { get; set; }
        public double quatW { get; set; }
        public FrameData1IMU(string csvLine)
        {
            string[] values = csvLine.Split(' ');
            time = getDouble(values, 1);
            frame = getInt(values, 2);
            accX = getDouble(values, 3);
            accY = getDouble(values, 4);
            accZ = getDouble(values, 5);
            gyrX = getDouble(values, 6);
            gyrY = getDouble(values, 7);
            gyrZ = getDouble(values, 8);
            magX = getDouble(values, 9);
            magY = getDouble(values, 10);
            magZ = getDouble(values, 11);

            laccX = getDouble(values, 12);
            laccY = getDouble(values, 13);
            laccZ = getDouble(values, 14);

            quatX = getDouble(values, 15);
            quatY = getDouble(values, 16);
            quatZ = getDouble(values, 17);
            quatW = getDouble(values, 18);
        }
    }
    public class FrameData2IMUs : FrameData
    {
        public override int numIMUs { get { return 2; } }
        public double angleX { get; set; }
        public double angleY { get; set; }
        public double angleZ { get; set; }
        public Vector3 angularVelocity { get; set; }
        public Vector3 angularAcceleration { get; set; }
        public FrameData2IMUs(string csvLine)
        {
            string[] values = csvLine.Split(' ');
            time = getDouble(values, 1);
            frame = getInt(values, 2);
            angleX = getDouble(values, 3);
            angleY = getDouble(values, 4);
            angleZ = getDouble(values, 5);
            angularVelocity = new Vector3(getFloat(values, 6), getFloat(values, 7),
                getFloat(values, 8));
            angularAcceleration = new Vector3(getFloat(values, 9), getFloat(values, 10),
                getFloat(values, 11));
        }
    }

    public class FrameDataSagital : FrameData
    {
        public override int numIMUs { get { return 4; } }
        public double rightAnkle { get; set; }
        public double rightHip { get; set; }
        public double rightKnee { get; set; }
        public FrameDataSagital(string csvLine)
        {
            string[] values = csvLine.Split(' ');
            time = getDouble(values, 1);
            frame = getInt(values, 2);
            rightAnkle = getDouble(values, 3);
            rightHip = getDouble(values, 4);
            rightKnee = getDouble(values, 5);
        }
    }
}
