using ScottPlot.Styles;
using System;
using System.Numerics;

namespace ibcdatacsharp.EFK_old
{
    public struct EKF
    {
        private Vector3 g;
        private Vector3 r;
        private float deltaT;
        //private Vector3 spectralDensity;
        private Matrix3x3? spectralNoiseCovarianceMatrix;
        private float? spectralNoise;
        public EKF(float deltaT, Vector3 spectralDensity, float magnetic_dip_angle , bool NED=true)
        {
            this.deltaT = deltaT;
            //this.spectralDensity = spectralDensity;
            this.spectralNoiseCovarianceMatrix = new Matrix3x3(
                spectralDensity.X, 0, 0,
                0, spectralDensity.Y, 0,
                0, 0, spectralDensity.Z
                );
            this.spectralNoise = null;
            if (NED)
            {
                g = new Vector3(0, 0, -1);
                r = new Vector3((float)Math.Cos(magnetic_dip_angle), 0, (float)Math.Sin(magnetic_dip_angle)) /
                    (float)Math.Sqrt(Math.Pow((float)Math.Cos(magnetic_dip_angle), 2) + Math.Pow((float)Math.Sin(magnetic_dip_angle), 2));
            }
            else
            {
                g = new Vector3(0, 0, 1);
                r = new Vector3((float)Math.Cos(magnetic_dip_angle), 0, -(float)Math.Sin(magnetic_dip_angle)) /
                    (float)Math.Sqrt(Math.Pow((float)Math.Cos(magnetic_dip_angle), 2) + Math.Pow((float)Math.Sin(magnetic_dip_angle), 2));
            }
        }
        public EKF(float deltaT, float spectralNoise, float magnetic_dip_angle, bool NED=true)
        {
            this.deltaT = deltaT;
            //this.spectralDensity = spectralDensity;
            this.spectralNoiseCovarianceMatrix = null;
            this.spectralNoise = spectralNoise;
            if (NED)
            {
                g = new Vector3(0, 0, -1);
                r = new Vector3((float)Math.Cos(magnetic_dip_angle), 0, (float)Math.Sin(magnetic_dip_angle)) /
                    (float)Math.Sqrt(Math.Pow((float)Math.Cos(magnetic_dip_angle), 2) + Math.Pow((float)Math.Sin(magnetic_dip_angle), 2));
            }
            else
            {
                g = new Vector3(0, 0, 1);
                r = new Vector3((float)Math.Cos(magnetic_dip_angle), 0, -(float)Math.Sin(magnetic_dip_angle)) /
                    (float)Math.Sqrt(Math.Pow((float)Math.Cos(magnetic_dip_angle), 2) + Math.Pow((float)Math.Sin(magnetic_dip_angle), 2));
            }
        }
        private Matrix4x4 buildF(Vector3 w)
        {
            return new Matrix4x4(
                1, -deltaT / 2 * w.X, -deltaT / 2 * w.Y, -deltaT / 2 * w.Z,
                deltaT / 2 * w.X, 1, deltaT / 2 * w.Z, -deltaT / 2 * w.Y,
                deltaT / 2 * w.Y, -deltaT / 2 * w.Z, 1, deltaT / 2 * w.X,
                deltaT / 2 * w.Z, deltaT / 2 * w.Y, -deltaT / 2 * w.X, 1
                );
        }
        private Matrix4x3 buildW(Quaternion q)
        {
            return new Matrix4x3(
                -q.X, -q.Y, -q.Z,
                q.W, -q.Z, q.Y,
                q.Z, q.W, -q.X,
                -q.Y, q.X, q.W
                ) * (deltaT / 2);
        }
        private Vector6 buildh(Quaternion q)
        {
            /*
            Vector6 full = new Vector6(
                g.X *(1/2 - q.Y * q.Y - q.Z * q.Z) + g.Y * (q.W * q.Z + q.X * q.Y) + g.Z * (q.X * q.Z - q.W * q.Y),
                g.X * (q.X * q.Y - q.W * q.Z),
                0,
                0,
                0,
                0);
            */
            Matrix3x3 C = Matrix3x3.CreateFromQuaternion(q);
            Vector3 a_hat = C * g;
            Vector3 m_hat = C * r;
            Vector6 result = new Vector6(a_hat, m_hat);
            return result;
        }
        private Matrix6x4 buildH(Quaternion q) // Lo he revisado no creo que este mal (tampoco estoy seguro)
        {
            Matrix6x4 result;

            result.M11 = g.Y * q.Z - g.Z * q.Y;
            result.M12 = g.Y * q.Y + g.Z * q.Z;
            result.M13 = -2 * g.X * q.Y + g.Y * q.X - g.Z * q.W;
            result.M14 = -2 * g.X * q.Z + g.Y * q.W + g.Z * q.X;

            result.M21 = -g.X * q.Z + g.Z * q.X;
            result.M22 = g.X * q.Y - 2 * g.Y * q.X + g.Z * q.W;
            result.M23 = g.X * q.X + g.Z * q.Z;
            result.M24 = -g.X * q.W - 2 * g.Y * q.Z + g.Z * q.Y;

            result.M31 = g.X * q.Y -g.Y * q.X;
            result.M32 = g.X * q.Z -g.Y * q.W - 2 * g.Z * q.X;
            result.M33 = g.X * q.W + g.Y * q.Z - 2 * g.Z * q.Y;
            result.M34 = g.X * q.X + g.Y * q.Y;

            result.M41 = r.Y * q.Z - r.Z * q.Y;
            result.M42 = r.Y * q.Y + r.Z * q.Z;
            result.M43 = -2 * r.X * q.Y + r.Y * q.X - r.Z * q.W;
            result.M44 = -2 * r.X * q.Z + r.Y * q.W + r.Z * q.X;

            result.M51 = -r.X * q.Z + r.Z * q.X;
            result.M52 = r.X * q.Y - 2 * r.Y * q.X + r.Z * q.W;
            result.M53 = r.X * q.X + r.Z * q.Z;
            result.M54 = -r.X * q.W - 2 * r.Y * q.Z + r.Z * q.Y;

            result.M61 = r.X * q.Y - r.Y * q.X;
            result.M62 = r.X * q.Z - r.Y * q.W - 2 * r.Z * q.X;
            result.M63 = r.X * q.W + r.Y * q.Z - 2 * r.Z * q.Y;
            result.M64 = r.X * q.X + r.Y * q.Y;

            return result;
        }
        private Matrix6x6 buildR(float noiseVarianceA, float noiseVarianceW)
        {
            Matrix3x3 M11 = noiseVarianceA * Matrix3x3.Identity();
            Matrix3x3 M12 = Matrix3x3.Zero();
            Matrix3x3 M21 = Matrix3x3.Zero();
            Matrix3x3 M22 = noiseVarianceW * Matrix3x3.Identity();
            Matrix6x6 result = new Matrix6x6(M11, M12, M21, M22);
            return result;
        }
        public Quaternion update(Quaternion q, Vector3 gyr, Vector3 acc, Vector3 mag)
        {
            #region prediction
            Matrix4x3 W = buildW(q);
            Matrix4x4 Q;
            if (spectralNoiseCovarianceMatrix == null)
            {
                Q = W * Matrix3x4.Transpose(W) * (float)spectralNoise;
            }
            else
            {
                Q = W * (Matrix3x3)spectralNoiseCovarianceMatrix * Matrix3x4.Transpose(W);
            }
            Matrix4x4 F = buildF(gyr);
            Matrix4x4 P_hat = F * Matrix4x4.CreateFromQuaternion(q) * Matrix4x4.Transpose(F) + Q;
            #endregion prediction
            #region correction
            Quaternion q_hat = q;
            Vector6 z = new Vector6(acc, mag);
            Vector6 h = buildh(q_hat);
            Vector6 v = z - h;
            Matrix6x4 H = buildH(q_hat);
            Matrix6x6 R = buildR(0.5f * 0.5f, 0.3f * 0.3f);
            Matrix6x6 S = H * P_hat * Matrix4x6.Transpose(H) + R;
            Matrix6x6 S_inv;
            if (Matrix6x6.Invert(S, out S_inv))
            {
                Matrix4x6 K = P_hat * Matrix4x6.Transpose(H) * S_inv;
                //Quaternion q = q_hat + K * v;
            }
            #endregion correction
            return new Quaternion();
        }
        public static void test()
        {
            Matrix3x3.test();
            Matrix4x3.test();
            Matrix4x6.test();
            Matrix6x4.test();
            Matrix6x6.test();
        }
    }
}
