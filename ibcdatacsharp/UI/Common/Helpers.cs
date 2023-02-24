//#define NORMALIZE_ANGLE_STATS

using System.Windows.Media;
using System.Windows;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Windows.Navigation;

namespace ibcdatacsharp.UI.Common
{
    public static class Helpers
    {
        // Devuelve el primer descendiente de tipo T de un objeto en el arbol xaml
        public static T GetChildOfType<T>(this DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }
        public static float NextFloat(float min, float max)
        {
            System.Random random = new System.Random();
            double val = (random.NextDouble() * (max - min) + min);
            return (float)val;
        }
        public static Vector3 ToEulerAngles(Quaternion q)
        {
            Vector3 angles = new();

            // roll / x
            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch / y
            double sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
            {
                angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
            }
            else
            {
                angles.Y = (float)Math.Asin(sinp);
            }

            // yaw / z
            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return angles;
        }
        public static float ToGs(float ms2)
        {
            return ms2 / 9.8f;
        }
        public static float ToDegrees(float radians)
        {
            float degrees = (180 / (float)Math.PI) * radians;
            return (degrees);
        }
        public static float ToRadians(float degrees)
        {
            float radians = ((float)Math.PI / 180) * degrees;
            return radians;
        }
        public static float NormalizeAngle(float angle)
        {
            /*
            while(angle > 360)
            {
                angle -= 360;
            }
            while(angle < -360)
            {
                angle += 360;
            }
            */
            return angle % 360;
        }
        // El angulo siempre esta en el rango -360 360
        public static float NormalizeAngle(float angle, float closeAngle)
        {
            angle = angle % 360;
            float dif = Math.Abs(angle - closeAngle);
            if (angle < 0)
            {
                float angleTest = angle + 360;
                float difTest = Math.Abs(angleTest - closeAngle);
                if (difTest < dif)
                {
                    return angleTest;
                }
                return angle;
            }
            if(angle > 0)
            {
                float angleTest = angle - 360;
                float difTest = Math.Abs(angleTest - closeAngle);
                if (difTest < dif)
                {
                    return angleTest;
                }
                return angle;
            }
            // Si llega aqui angle == 0
            if(closeAngle < -180)
            {
                return -360;
            }
            if(closeAngle > 180)
            {
                return 360;
            }
            return 0;
        }
#if NORMALIZE_ANGLE_STATS
        private static int NormalizeAngleWhileIter = 0;
        private static int NormalizeAngleCalls = 0;
#endif
        // Forma general. El angulo se puede salir del rango -360 360
        public static float NormalizeAngleLoop(float angle, float closeAngle)
        {
#if NORMALIZE_ANGLE_STATS
            NormalizeAngleCalls++;
#endif
            float dif = Math.Abs(angle - closeAngle);
            float angleRes = angle;
            float difRes = dif;
            float angleTest = angle;
            while(angleTest <= 0)
            {
#if NORMALIZE_ANGLE_STATS
                NormalizeAngleWhileIter++;
#endif
                angleTest += 360;
                float difTest = Math.Abs(angleTest - closeAngle);
                if(difTest < difRes)
                {
                    angleRes = angleTest;
                    difRes = difTest;
                }
                else if(difTest > dif)
                {
                    break;
                }
            }
            angleTest = angle;
            while (angleTest >= 0)
            {
#if NORMALIZE_ANGLE_STATS
                NormalizeAngleWhileIter++;
#endif
                angleTest -= 360;
                float difTest = Math.Abs(angleTest - closeAngle);
                if (difTest < difRes)
                {
                    angleRes = angleTest;
                    difRes = difTest;
                }
                else if (difTest > dif)
                {
                    break;
                }
            }
#if NORMALIZE_ANGLE_STATS
            if(NormalizeAngleCalls % 1000 == 0)
            {
                Trace.WriteLine("average while iterations per call: " + (float)NormalizeAngleWhileIter / NormalizeAngleCalls);
            }
#endif
            return angleRes;
        }
        public static void NormalizeAngleTest()
        {
            float[] angles = new float[] {10, -180, 320, -250};
            float[] prevs = new float[] {20, 0, 280, 0};
            float[] res = new float[] { 10, -180, 320, 110 };
            Trace.WriteLine("Normalize Angle Test");
            for(int i = 0; i < angles.Length; i++)
            {
                float a = NormalizeAngle(angles[i], prevs[i]);
                if(a != res[i])
                {
                    Trace.WriteLine("res = " + a + ", angle = " + angles[i] + ", prev = " + prevs[i]);
                }
            }
        }
        public static Vector3 AngularVelocityFromQuaternions(Quaternion q1, Quaternion q2, double dt)
        {


            Vector3 v = new Vector3();
            v.X = (float)((2 / dt) * (q1.W * q2.X - q1.X * q2.W - q1.Y * q2.Z + q1.Z * q2.Y));
            v.Y = (float)((2 / dt) * (q1.W * q2.Y + q1.X * q2.Z - q1.Y * q2.W - q1.Z * q2.X));
            v.Z = (float)((2 / dt) * (q1.W * q2.Z - q1.X * q2.Y + q1.Y * q2.X - q1.Z * q2.W));

            return v;

        }
        public static float AngularVelocityFromDegrees(float angle1, float angle0, float dt)
        {
            float angle1Rad = ToRadians(angle1);
            float angle0Rad = ToRadians(angle0);
            return AngularVelocity(angle1Rad, angle0Rad, dt);
        }
        public static float AngularVelocity(float angle1, float angle0, float dt)
        {
            return (angle1 - angle0) / dt;
        }
        public static Vector3 AngularAcceleration(Vector3 w1, Vector3 w0, float dt)
        {
            return (w1 - w0) / dt;
        }
        public static float AngularAcceleration(float w1, float w0, float dt)
        {
            return (w1 - w0) / dt;
        }
        public static void printDict(Dictionary<string, WisewalkSDK.Device> dict)
        {
            foreach (var kvp in dict)
            {
                string s = string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value.Id);
                Trace.WriteLine(s);
            }
        }
        public static void printArray(float[,] array)
        {
            Trace.WriteLine("array");
            for(int i = 0; i < array.GetLength(0); i++)
            {
                string temp = "[";
                for(int j = 0; j < array.GetLength(1); j++)
                {
                    temp += array[i, j] + ", ";
                }
                temp += "]";
                Trace.WriteLine(temp);
            }
        }
        public static bool NearlyEqual(float a, float b, float epsilon = 0.1f)
        {
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);
            float diff = Math.Abs(a - b);

            if (a == b)
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || absA + absB < float.MinValue)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * float.MaxValue);
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }
        public static void printDevicesConnected()
        {
            Trace.WriteLine("devices connected:");
            var devicesConnected = ((MainWindow)Application.Current.MainWindow).api.GetDevicesConnected();
            foreach (KeyValuePair<string, WisewalkSDK.Device> entry in devicesConnected)
            {
                Trace.WriteLine("handler: " + entry.Key);
                Trace.WriteLine(entry.Value.Id);
            }
        }
        public static void callWhenNavigated(Frame frame, Action f)
        {
            if(frame.Content == null)
            {
                frame.Navigated += (sender, args) =>
                {
                    f();
                };
            }
            else
            {
                f();
            }
        }
        public static float randomFloat(float min, float max)
        {
            Random random = new Random();
            float f = random.NextSingle();
            f = f * (max - min);
            return f + min;
        }
        public static Quaternion random_quaternion()
        {
            float x, y, z, u, v, w, s;
            do { x = randomFloat(-1,1); y = randomFloat(-1, 1); z = x * x + y * y; } while (z > 1);
            do { u = randomFloat(-1, 1); v = randomFloat(-1, 1); w = u * u + v * v; } while (w > 1);
            s = (float)Math.Sqrt((1 - z) / w);
            return new Quaternion(x, y, s * u, s * v);
        }
        public static void print(List<WisewalkSDK.QuatSensor> quats)
        {
            for(int i = 0; i < quats.Count; i++)
            {
                Trace.WriteLine(quats[i].W + " " + quats[i].X + " " + quats[i].Y + " " +
                    quats[i].Z);
            }
        }
    }
}