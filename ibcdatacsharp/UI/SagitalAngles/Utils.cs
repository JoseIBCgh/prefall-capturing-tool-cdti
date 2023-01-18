using System;
using System.Diagnostics;
using System.Numerics;

namespace ibcdatacsharp.UI.SagitalAngles
{
    public static class Utils
    {
        public const double M_PI = Math.PI;
        public const double M_2PI = Math.PI * 2;
        private static double fmod(double x, double y)
        {
            return x % y;
        }
        private static double constrainAngle(double x)
        {
            x = fmod(x + M_PI, M_2PI);
            if (x < 0)
                x += M_2PI;
            return x - M_PI;
        }
        // convert to [-360,360]
        private static double angleConv(double angle)
        {
            return fmod(constrainAngle(angle), M_2PI);
        }
        private static double angleDiff(double a, double b)
        {
            double dif = fmod(b - a + M_PI, M_2PI);
            if (dif < 0)
                dif += M_2PI;
            return dif - M_PI;
        }
        public static double unwrap(double previousAngle, double newAngle)
        {
            return previousAngle - angleDiff(newAngle, angleConv(previousAngle));
        }
        public static int updateAngleMode(int prevAngleMode, double prevAngle, double newAngle)
        {
            double angleDifference = angleDiff(prevAngle, newAngle);
            /*
            if( angleDifference > 1.57 )
            {
                qDebug() << angleDifference;
            }
            */

            if (angleDifference >= (M_PI - (M_PI / 180 * 0.5)))
            {
                if (prevAngle > newAngle)
                {
                    return 1; // lastjumpy > jumpy, add nothing
                }
                else
                {
                    return 0; // return mode 0
                }
            }

            return prevAngleMode;
        }
        public enum RotSeq { zyx, zyz, zxy, zxz, yxz, yxy, yzx, yzy, xyz, xyx, xzy, xzx };
        private static void twoaxisrot(double r11, double r12, double r21, double r31, double r32, ref double[] res)
        {
            res[0] = Math.Atan2(r11, r12);
            res[1] = Math.Acos(r21);
            res[2] = Math.Atan2(r31, r32);
        }
        private static void threeaxisrot(double r11, double r12, double r21, double r31, double r32, ref double[] res)
        {
            res[0] = Math.Atan2(r31, r32);
            res[1] = Math.Asin(r21);
            res[2] = Math.Atan2(r11, r12);
        }
        public static void quaternion2euler(Quaternion q, ref double[] res, RotSeq rotSeq)
        {
            double q_x, q_y, q_z, q_w;
            q_x = q.X;
            q_y = q.Y;
            q_z = q.Z;
            q_w = q.W;
            switch (rotSeq)
            {
                case RotSeq.zyx:
                    threeaxisrot(2 * (q_x * q_y + q_w * q_z),
                                   q_w * q_w + q_x * q_x - q_y * q_y - q_z * q_z,
                                  -2 * (q_x * q_z - q_w * q_y),
                                   2 * (q_y * q_z + q_w * q_x),
                                   q_w * q_w - q_x * q_x - q_y * q_y + q_z * q_z,
                                   ref res);
                    break;

                case RotSeq.zyz:
                    twoaxisrot(2 * (q_y * q_z - q_w * q_x),
                                 2 * (q_x * q_z + q_w * q_y),
                                 q_w * q_w - q_x * q_x - q_y * q_y + q_z * q_z,
                                 2 * (q_y * q_z + q_w * q_x),
                                -2 * (q_x * q_z - q_w * q_y),
                                ref res);
                    break;

                case RotSeq.zxy:
                    threeaxisrot(-2 * (q_x * q_y - q_w * q_z),
                                    q_w * q_w - q_x * q_x + q_y * q_y - q_z * q_z,
                                    2 * (q_y * q_z + q_w * q_x),
                                   -2 * (q_x * q_z - q_w * q_y),
                                    q_w * q_w - q_x * q_x - q_y * q_y + q_z * q_z,
                                    ref res);
                    break;

                case RotSeq.zxz:
                    twoaxisrot(2 * (q_x * q_z + q_w * q_y),
                                -2 * (q_y * q_z - q_w * q_x),
                                 q_w * q_w - q_x * q_x - q_y * q_y + q_z * q_z,
                                 2 * (q_x * q_z - q_w * q_y),
                                 2 * (q_y * q_z + q_w * q_x),
                                 ref res);
                    break;

                case RotSeq.yxz:
                    threeaxisrot(2 * (q_x * q_z + q_w * q_y),
                                   q_w * q_w - q_x * q_x - q_y * q_y + q_z * q_z,
                                  -2 * (q_y * q_z - q_w * q_x),
                                   2 * (q_x * q_y + q_w * q_z),
                                   q_w * q_w - q_x * q_x + q_y * q_y - q_z * q_z,
                                   ref res);
                    break;

                case RotSeq.yxy:
                    twoaxisrot(2 * (q_x * q_y - q_w * q_z),
                                 2 * (q_y * q_z + q_w * q_x),
                                 q_w * q_w - q_x * q_x + q_y * q_y - q_z * q_z,
                                 2 * (q_x * q_y + q_w * q_z),
                                -2 * (q_y * q_z - q_w * q_x),
                                ref res);
                    break;

                case RotSeq.yzx:
                    threeaxisrot(-2 * (q_x * q_z - q_w * q_y),
                                    q_w * q_w + q_x * q_x - q_y * q_y - q_z * q_z,
                                    2 * (q_x * q_y + q_w * q_z),
                                   -2 * (q_y * q_z - q_w * q_x),
                                    q_w * q_w - q_x * q_x + q_y * q_y - q_z * q_z,
                                    ref res);
                    break;

                case RotSeq.yzy:
                    twoaxisrot(2 * (q_y * q_z + q_w * q_x),
                                -2 * (q_x * q_y - q_w * q_z),
                                 q_w * q_w - q_x * q_x + q_y * q_y - q_z * q_z,
                                 2 * (q_y * q_z - q_w * q_x),
                                 2 * (q_x * q_y + q_w * q_z),
                                 ref res);
                    break;

                case RotSeq.xyz:
                    threeaxisrot(-2 * (q_y * q_z - q_w * q_x),
                                  q_w * q_w - q_x * q_x - q_y * q_y + q_z * q_z,
                                  2 * (q_x * q_z + q_w * q_y),
                                 -2 * (q_x * q_y - q_w * q_z),
                                  q_w * q_w + q_x * q_x - q_y * q_y - q_z * q_z,
                                  ref res);
                    break;

                case RotSeq.xyx:
                    twoaxisrot(2 * (q_x * q_y + q_w * q_z),
                                -2 * (q_x * q_z - q_w * q_y),
                                 q_w * q_w + q_x * q_x - q_y * q_y - q_z * q_z,
                                 2 * (q_x * q_y - q_w * q_z),
                                 2 * (q_x * q_z + q_w * q_y),
                                 ref res);
                    break;

                case RotSeq.xzy:
                    threeaxisrot(2 * (q_y * q_z + q_w * q_x),
                                   q_w * q_w - q_x * q_x + q_y * q_y - q_z * q_z,
                                  -2 * (q_x * q_y - q_w * q_z),
                                   2 * (q_x * q_z + q_w * q_y),
                                   q_w * q_w + q_x * q_x - q_y * q_y - q_z * q_z,
                                   ref res);
                    break;

                case RotSeq.xzx:
                    twoaxisrot(2 * (q_x * q_z - q_w * q_y),
                                 2 * (q_x * q_y + q_w * q_z),
                                 q_w * q_w + q_x * q_x - q_y * q_y - q_z * q_z,
                                 2 * (q_x * q_z + q_w * q_y),
                                -2 * (q_x * q_y - q_w * q_z),
                                ref res);
                    break;
                default:
                    Trace.WriteLine("Unknown rotation sequence");
                    break;
            }
        }
        public static double rad2deg(double rad)
        {
            return rad * 180.0 / M_PI;
        }
    }
}
