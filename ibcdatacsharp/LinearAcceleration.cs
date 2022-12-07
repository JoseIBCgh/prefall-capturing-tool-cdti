using ibcdatacsharp.UI.Graphs;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Globalization;
using System;
using System.Diagnostics;

namespace ibcdatacsharp
{
    public struct LinearAcceleration
    {
        private static Vector3 g = new Vector3(0, 0, -1);
        private static Vector3 quaternionRotateVector(Quaternion q, Vector3 v)
        {
            Quaternion qvq = Quaternion.Conjugate(q) * new Quaternion(v, 1) * q;
            return new Vector3(qvq.X, qvq.Y, qvq.Z);
        }
        public static Vector3 calcLinAcc(Quaternion q, Vector3 acc)
        {
            q = Quaternion.Normalize(q);
            Vector3 gRot = quaternionRotateVector(q, g);
            return gRot - acc;
        }
        public static void test(string filename = "C:\\Temp\\a1.csv")
        {
            using (var reader = new StreamReader(filename))
            {
                int linesToSkip = 1;
                for (int i = 0; i < linesToSkip; i++)
                {
                    reader.ReadLine();
                }
                float maxError = 0;
                float totalError = 0;
                int numLines = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    float accx = float.Parse(values[3], CultureInfo.InvariantCulture);
                    float accy = float.Parse(values[4], CultureInfo.InvariantCulture);
                    float accz = float.Parse(values[5], CultureInfo.InvariantCulture);
                    float qx = float.Parse(values[16], CultureInfo.InvariantCulture);
                    float qy = float.Parse(values[17], CultureInfo.InvariantCulture);
                    float qz = float.Parse(values[18], CultureInfo.InvariantCulture);
                    float qw = float.Parse(values[15], CultureInfo.InvariantCulture);
                    float laccx = float.Parse(values[19], CultureInfo.InvariantCulture);
                    float laccy = float.Parse(values[20], CultureInfo.InvariantCulture);
                    float laccz = float.Parse(values[21], CultureInfo.InvariantCulture);
                    Quaternion qsensor = new Quaternion(qx, qy, qz, qw);
                    qsensor = Quaternion.Conjugate(qsensor);
                    Vector3 acc = new Vector3(accx, accy, accz);
                    Vector3 lacc = new Vector3(laccx, laccy, laccz);
                    Vector3 lacc_cal = calcLinAcc(qsensor, acc);
                    float diference =  Math.Abs(lacc.X - lacc_cal.X) + Math.Abs(lacc.Y - lacc_cal.Y) +
                        Math.Abs(lacc.Z - lacc_cal.Z);
                    float total = Math.Abs(lacc.X + lacc.Y + lacc.Z);
                    float error = diference / total;
                    totalError += error;
                    numLines++;
                    if(error > maxError)
                    {
                        maxError = error;
                    }
                    Trace.WriteLine("teorico " + lacc + " calculado " + lacc_cal);
                }
                Trace.WriteLine("Max error " + (maxError * 100).ToString() + " %");
                float errorMedio = totalError / numLines;
                Trace.WriteLine("Error medio " + (errorMedio * 100).ToString() + " %");
            }
        }
    }
}
