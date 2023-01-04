using ibcdatacsharp.UI.Common;
using ibcdatacsharp.UI.Graphs;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Diagnostics;
using System.Numerics;

namespace ibcdatacsharp.EFK
{
    public class EKF
    {
        private MathNet.Numerics.LinearAlgebra.Vector<float> g;
        private MathNet.Numerics.LinearAlgebra.Vector<float> r;
        private float deltaT;
        private Matrix<float>? spectralNoiseCovarianceMatrix;
        private float? spectralNoise;
        public Matrix<float> H { get; private set; }
        public MathNet.Numerics.LinearAlgebra.Vector<float> h { get; private set; }

        public EKF(float deltaT, MathNet.Numerics.LinearAlgebra.Vector<float> spectralDensity, 
            float magnetic_dip_angle, bool NED = true)
        {
            magnetic_dip_angle = Helpers.ToRadians(magnetic_dip_angle);
            this.deltaT = deltaT;
            //this.spectralDensity = spectralDensity;
            this.spectralNoiseCovarianceMatrix = Matrix<float>.Build.DenseOfArray(
                new float[,] {
                { spectralDensity[0], 0, 0 },
                { 0, spectralDensity[1], 0 },
                { 0, 0, spectralDensity[2] }
                }
                );
            this.spectralNoise = null;
            if (NED)
            {
                //g = new Vector3(0, 0, -1);
                g = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Dense(new float[] { 0, 0, -1 });
                r = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Dense(new float[]
                {
                    (float)Math.Cos(magnetic_dip_angle), 0, (float)Math.Sin(magnetic_dip_angle)
                }) /
                (float)Math.Sqrt(Math.Pow((float)Math.Cos(magnetic_dip_angle), 2) + Math.Pow((float)Math.Sin(magnetic_dip_angle), 2));
            }
            else
            {
                g = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Dense(new float[] { 0, 0, 1 });
                r = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Dense(new float[]
                {
                    (float)Math.Cos(magnetic_dip_angle), 0, -(float)Math.Sin(magnetic_dip_angle)
                }) /
                (float)Math.Sqrt(Math.Pow((float)Math.Cos(magnetic_dip_angle), 2) + Math.Pow((float)Math.Sin(magnetic_dip_angle), 2));
            }
        }
        public EKF(float deltaT, float spectralNoise, float magnetic_dip_angle, bool NED = true)
        {
            magnetic_dip_angle = Helpers.ToRadians(magnetic_dip_angle);
            this.deltaT = deltaT;
            //this.spectralDensity = spectralDensity;
            this.spectralNoiseCovarianceMatrix = null;
            this.spectralNoise = spectralNoise;
            if (NED)
            {
                g = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Dense(new float[] { 0, 0, -1 });
                r = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Dense(new float[]
                {
                    (float)Math.Cos(magnetic_dip_angle), 0, (float)Math.Sin(magnetic_dip_angle)
                }) /
                (float)Math.Sqrt(Math.Pow((float)Math.Cos(magnetic_dip_angle), 2) + Math.Pow((float)Math.Sin(magnetic_dip_angle), 2));
            }
            else
            {
                g = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Dense(new float[] { 0, 0, 1 });
                r = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Dense(new float[]
                {
                    (float)Math.Cos(magnetic_dip_angle), 0, -(float)Math.Sin(magnetic_dip_angle)
                }) /
                (float)Math.Sqrt(Math.Pow((float)Math.Cos(magnetic_dip_angle), 2) + Math.Pow((float)Math.Sin(magnetic_dip_angle), 2));
            }
        }
        private Quaternion buildqhat(Quaternion q, 
            MathNet.Numerics.LinearAlgebra.Vector<float> w)
        {
            return new Quaternion(
                q.X + deltaT / 2 * w[0] * q.W - deltaT / 2 * w[1] * q.Z + deltaT / 2 * w[2] * q.Y,
                q.Y + deltaT / 2 * w[0] * q.Z + deltaT / 2 * w[1] * q.W - deltaT / 2 * w[2] * q.X,
                q.Z - deltaT / 2 * w[0] * q.Y + deltaT / 2 * w[1] * q.X + deltaT / 2 * w[2] * q.W,
                q.W - deltaT / 2 * w[0] * q.X - deltaT / 2 * w[1] * q.Y - deltaT / 2 * w[2] * q.Z
                );
        }
        public Matrix<float> buildF(MathNet.Numerics.LinearAlgebra.Vector<float> w)
        {

            return Matrix<float>.Build.DenseOfArray(
                new float[,]
                {
                    { 1, -deltaT / 2 * w[0], -deltaT / 2 * w[1], -deltaT / 2 * w[2] },
                    { deltaT / 2 * w[0], 1, deltaT / 2 * w[2], -deltaT / 2 * w[1] },
                    { deltaT / 2 * w[1], -deltaT / 2 * w[2], 1, deltaT / 2 * w[0] },
                    { deltaT / 2 * w[2], deltaT / 2 * w[1], -deltaT / 2 * w[0], 1 }
                }
                );
        }
        private Matrix<float> buildW(Quaternion q)
        {
            return Matrix<float>.Build.DenseOfArray(
                new float[,]
                {
                {-q.X, -q.Y, -q.Z },
                {q.W, -q.Z, q.Y },
                { q.Z, q.W, -q.X },
                { -q.Y, q.X, q.W }
                }
                ) * (deltaT / 2);
        }
        private MathNet.Numerics.LinearAlgebra.Vector<float> buildh(Quaternion q)
        {
            Matrix<float> C = CreateFromQuaternion3x3(q);
            MathNet.Numerics.LinearAlgebra.Vector<float> a_hat = C * g;
            MathNet.Numerics.LinearAlgebra.Vector<float> m_hat = C * r;
            MathNet.Numerics.LinearAlgebra.Vector<float> result = CreateFromVectors(
                new MathNet.Numerics.LinearAlgebra.Vector<float>[] {a_hat, m_hat}
                );
            return result;
        }
        public static Matrix<float> CreateFromQuaternion3x3(Quaternion quaternion)
        {
            Matrix4x4 m = Matrix4x4.CreateFromQuaternion(quaternion);

            Matrix<float> result = Matrix<float>.Build.DenseOfArray(
                new float[,]
                {
                { m.M11, m.M12, m.M13 },
                { m.M21, m.M22, m.M23 },
                { m.M31, m.M32, m.M33 }
                }
                );

            return result;
        }
        public static Matrix<float> CreateFromQuaternion4x4(Quaternion quaternion)
        {
            Matrix4x4 m = Matrix4x4.CreateFromQuaternion(quaternion);

            Matrix<float> result = Matrix<float>.Build.DenseOfArray(
                new float[,]
                {
                { m.M11, m.M12, m.M13, m.M14 },
                { m.M21, m.M22, m.M23, m.M24 },
                { m.M31, m.M32, m.M33, m.M34 },
                { m.M41, m.M42, m.M43, m.M44 }
                }
                );

            return result;
        }
        public static MathNet.Numerics.LinearAlgebra.Vector<float> CreateFromVectors
            (MathNet.Numerics.LinearAlgebra.Vector<float>[] vectors)//Funciona bien
        {
            //Trace.WriteLine("CreateFromVectors");
            int length = 0;
            foreach (var vector in vectors)
            {
                //Trace.WriteLine(vector);
                length += vector.Count;
            }
            float[] array = new float[length];
            int array_i = 0;
            for(int i = 0; i < vectors.Length; i++)
            {
                for(int j = 0; j < vectors[i].Count; j++)
                {
                    array[array_i] = vectors[i][j];
                    array_i++;
                }
            }
            //Trace.WriteLine("result");
            MathNet.Numerics.LinearAlgebra.Vector<float> result = 
                MathNet.Numerics.LinearAlgebra.Vector<float>.Build.DenseOfArray(array);
            //Trace.WriteLine(result);
            return result;
        }
        public static MathNet.Numerics.LinearAlgebra.Vector<float> ToVector(Quaternion q)
        {
            return MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Dense(new float[]
            {
                q.X, q.Y, q.Z, q.W
            });
        }
        public static Quaternion ToQuaternion(MathNet.Numerics.LinearAlgebra.Vector<float> v)
        {
            return new Quaternion(v[0], v[1], v[2], v[3]);
        }
        private Matrix<float> buildH(Quaternion q)
        {
            float[,] array = new float[,]
                {
                    {
                        g[1] * q.Z - g[2] * q.Y,
                        g[1] * q.Y + g[2] * q.Z,
                        -2 * g[0] * q.Y + g[1] * q.X - g[2] * q.W,
                        -2 * g[0] * q.Z + g[1] * q.W + g[2] * q.X
                    },
                    {
                        -g[0] * q.Z + g[2] * q.X,
                        g[0] * q.Y - 2 * g[1] * q.X + g[2] * q.W,
                        g[0] * q.X + g[2] * q.Z,
                        -g[0] * q.W - 2 * g[1] * q.Z + g[2] * q.Y
                    },
                    {
                        g[0] * q.Y - g[1] * q.X,
                        g[0] * q.Z - g[1] * q.W - 2 * g[2] * q.X,
                        g[0] * q.W + g[1] * q.Z - 2 * g[2] * q.Y,
                        g[0] * q.X + g[1] * q.Y
                    },
                    {
                        r[1] * q.Z - r[2] * q.Y,
                        r[1] * q.Y + r[2] * q.Z,
                        -2 * r[0] * q.Y + r[1] * q.X - r[2] * q.W,
                        -2 * r[0] * q.Z + r[1] * q.W + r[2] * q.X
                    },
                    {
                        -r[0] * q.Z + r[2] * q.X,
                        r[0] * q.Y - 2 * r[1] * q.X + r[2] * q.W,
                        r[0] * q.X + r[2] * q.Z,
                        -r[0] * q.W - 2 * r[1] * q.Z + r[2] * q.Y
                    },
                    {
                        r[0] * q.Y - r[1] * q.X,
                        r[0] * q.Z - r[1] * q.W - 2 * r[2] * q.X,
                        r[0] * q.W + r[1] * q.Z - 2 * r[2] * q.Y,
                        r[0] * q.X + r[1] * q.Y
                    }
                };
            Matrix<float> result = Matrix<float>.Build.DenseOfArray(array) * 2;
            return result;
        }
        private Matrix<float> buildR(float noiseVarianceA, float noiseVarianceW)
        {
            Matrix<float> M11 = Matrix<float>.Build.DenseDiagonal(3, noiseVarianceA);
            Matrix<float> M12 = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> M21 = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> M22 = Matrix<float>.Build.DenseDiagonal(3, noiseVarianceW);
            Matrix<float> result = Matrix<float>.Build.DenseOfMatrixArray(
                new Matrix<float>[,]
                {
                    {M11, M12 },
                    {M21, M22 }
                }
                );
            return result;
        }
        public Quaternion update(Quaternion q, MathNet.Numerics.LinearAlgebra.Vector<float> gyr,
            MathNet.Numerics.LinearAlgebra.Vector<float> acc, MathNet.Numerics.LinearAlgebra.Vector<float> mag)
        {
            #region prediction
            Matrix<float> W = buildW(q);
            Matrix<float> Q;
            if (spectralNoiseCovarianceMatrix == null)
            {
                Q = W * W.Transpose() * (float)spectralNoise;
            }
            else
            {
                Q = W * spectralNoiseCovarianceMatrix * W.Transpose();
            }
            Matrix<float> F = buildF(gyr);
            Matrix<float> P_hat = F * CreateFromQuaternion4x4(q) * F.Transpose() + Q;
            Quaternion q_hat = buildqhat(q, gyr);
            #endregion prediction
            #region correction
            MathNet.Numerics.LinearAlgebra.Vector<float> z = CreateFromVectors(
                new MathNet.Numerics.LinearAlgebra.Vector<float>[]
                {
                    acc, mag
                });
            h = buildh(q_hat);
            MathNet.Numerics.LinearAlgebra.Vector<float> v = z - h;
            H = buildH(q_hat);
            Matrix<float> R = buildR(0.5f * 0.5f, 0.3f * 0.3f);
            Matrix<float> S = H * P_hat * H.Transpose() + R;
            Matrix<float> K = P_hat * H.Transpose() * S.Inverse();
            MathNet.Numerics.LinearAlgebra.Vector<float> q_vector = ToVector(q_hat) + K * v;
            Matrix<float> P = (Matrix<float>.Build.DenseIdentity(4) - K * H) * P_hat;
            q_vector = q_vector.Normalize(2); //2???
            #endregion correction
            return ToQuaternion(q_vector);
        }
        public static void test()
        {
            void testh() //Funciona bien
            {
                EKF ekf = new EKF(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED: false);
                Quaternion q0 = new Quaternion(0.7071f, 0.0f, -0.7071f, 0.0f);
                Trace.WriteLine("h");
                Trace.WriteLine("expected:");
                Trace.WriteLine("[-1.00000000e+00  0.00000000e+00  2.22044605e-16  8.66025404e-01 0.00000000e+00 -5.00000000e-01]");
                Trace.WriteLine("calculated:");
                Trace.WriteLine(ekf.buildh(q0));
            }
            void testH()
            {
                EKF ekf = new EKF(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED: false);
                Quaternion q0 = new Quaternion(0.7071f, 0.0f, -0.7071f, 0.0f);
                Trace.WriteLine("H");
                Trace.WriteLine("expected:");
                Trace.WriteLine(@"
[[-1.41421356 -0.          1.41421356  0.        ]
 [-0.         -1.41421356  0.          1.41421356]
 [-0.          0.         -2.82842712  0.        ]
 [ 1.22474487  0.          0.18946869  0.        ]
 [ 0.          0.51763809  0.         -1.93185165]
 [-0.70710678  0.          3.15659652  0.        ]]
");
                Trace.WriteLine("calculated:");
                Trace.WriteLine(ekf.buildH(q0));
            }
            void testF() // Funciona bien
            {
                EKF ekf = new EKF(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED: false);
                Quaternion q0 = new Quaternion(0.7071f, 0.0f, -0.7071f, 0.0f);
                MathNet.Numerics.LinearAlgebra.Vector<float> gyr =
                    MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Dense(new float[]{
                196.55102539f, 555.07427979f, -756.51245117f
                    });
                Trace.WriteLine("F");
                Trace.WriteLine("expected:");
                Trace.WriteLine(@"
[[ 1.         -0.98275513 -2.7753714   3.78256226]
 [ 0.98275513  1.         -3.78256226 -2.7753714 ]
 [ 2.7753714   3.78256226  1.          0.98275513]
 [-3.78256226  2.7753714  -0.98275513  1.        ]]
");
                Trace.WriteLine("calculated:");
                Trace.WriteLine(ekf.buildF(gyr));
            }
            void testFull()
            {
                Trace.WriteLine("start EKF test");
                EKF ekf = new EKF(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED:false);
                Quaternion q0 = new Quaternion(0.7071f, 0.0f, -0.7071f, 0.0f);
                MathNet.Numerics.LinearAlgebra.Vector<float> gyr =
                    MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Dense(new float[]{
                196.55102539f, 555.07427979f, -756.51245117f
                    });
                MathNet.Numerics.LinearAlgebra.Vector<float> acc =
                    MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Dense(new float[]{
                1.98162329f, -0.37048489f, -0.25054383f
                    });
                MathNet.Numerics.LinearAlgebra.Vector<float> mag =
                    MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Dense(new float[]{
                 -9.86029911f, -16.94231987f, 37.22705078f
                    });
                Quaternion q = ekf.update(q0, gyr, acc, mag);
                Trace.WriteLine("end EKF test");
                Trace.WriteLine("calculated:");
                Trace.WriteLine(q);
                Trace.WriteLine("expected:");
                Trace.WriteLine("[ 0.4988753   0.38271055  0.77311449 -0.08336696]");
            }
            void testToVQConversion() // Funciona bien
            {
                EKF ekf = new EKF(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED: false);
                Quaternion q0 = new Quaternion(0.7071f, 0.0f, -0.7071f, 0.0f);
                Trace.WriteLine("original quaternion: ");
                Trace.WriteLine(q0);
                MathNet.Numerics.LinearAlgebra.Vector<float> v = ToVector(q0);
                Trace.WriteLine("vector: ");
                Trace.WriteLine(v);
                Quaternion q = ToQuaternion(v);
                Trace.WriteLine("quaternion converted: ");
                Trace.WriteLine(q);
            }
            testFull();
            //testToVQConversion();
            //testh();
            testH();
            //testF();
        }
    }
}
