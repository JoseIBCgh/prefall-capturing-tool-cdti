using ibcdatacsharp.UI.Common;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Diagnostics;
using System.Numerics;

namespace ibcdatacsharp.EKF
{
    public class EKF
    {
        private MathNet.Numerics.LinearAlgebra.Vector<float> g;
        private MathNet.Numerics.LinearAlgebra.Vector<float> r;
        private float deltaT;
        private Matrix<float>? spectralNoiseCovarianceMatrix;
        private float? spectralNoise;
        private float var_acc;
        private float var_mag;
        private Matrix<float> P;
        public Matrix<float> H { get; private set; }
        public MathNet.Numerics.LinearAlgebra.Vector<float> h { get; private set; }

        public EKF(float deltaT, MathNet.Numerics.LinearAlgebra.Vector<float> spectralDensity, 
            float magnetic_dip_angle, bool NED = true, float var_acc = 0.5f * 0.5f, 
            float var_mag = 0.8f * 0.8f)
        {
            P = Matrix<float>.Build.DenseIdentity(4);
            magnetic_dip_angle = Helpers.ToRadians(magnetic_dip_angle);
            this.var_acc = var_acc;
            this.var_mag = var_mag;
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
                    0, (float)Math.Cos(magnetic_dip_angle), -(float)Math.Sin(magnetic_dip_angle)
                }) /
                (float)Math.Sqrt(Math.Pow((float)Math.Cos(magnetic_dip_angle), 2) + Math.Pow((float)Math.Sin(magnetic_dip_angle), 2));
            }
        }
        public EKF(float deltaT, float spectralNoise, float magnetic_dip_angle, bool NED = true, 
            float var_acc = 0.5f * 0.5f, float var_mag = 0.8f * 0.8f)
        {
            P = Matrix<float>.Build.DenseIdentity(4);
            magnetic_dip_angle = Helpers.ToRadians(magnetic_dip_angle);
            this.deltaT = deltaT;
            this.var_acc = var_acc;
            this.var_mag = var_mag;
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
                    0, (float)Math.Cos(magnetic_dip_angle), -(float)Math.Sin(magnetic_dip_angle)
                }) /
                (float)Math.Sqrt(Math.Pow((float)Math.Cos(magnetic_dip_angle), 2) + Math.Pow((float)Math.Sin(magnetic_dip_angle), 2));
            }
        }
        private Matrix<float> buildOmega(MathNet.Numerics.LinearAlgebra.Vector<float> v)
        {
            return Matrix<float>.Build.DenseOfArray(
                new float[,]
                {
                    { 0, -v[0], -v[1], -v[2] },
                    { v[0], 0, v[2], -v[1] },
                    { v[1], -v[2], 0, v[0] },
                    { v[2], v[1], -v[0], 0 }
                }
                );
        }
        private Quaternion buildqhat(Quaternion q, 
            MathNet.Numerics.LinearAlgebra.Vector<float> w, float dt)
        {
            Matrix<float> omega = buildOmega(w);
            MathNet.Numerics.LinearAlgebra.Vector<float> qhat_vector =
                (Matrix<float>.Build.DiagonalIdentity(4) + (0.5f * dt * omega)) * ToVector(q);
            return ToQuaternion(qhat_vector);
            /*
            return new Quaternion(
                q.X + deltaT / 2 * w[0] * q.W - deltaT / 2 * w[1] * q.Z + deltaT / 2 * w[2] * q.Y,
                q.Y + deltaT / 2 * w[0] * q.Z + deltaT / 2 * w[1] * q.W - deltaT / 2 * w[2] * q.X,
                q.Z - deltaT / 2 * w[0] * q.Y + deltaT / 2 * w[1] * q.X + deltaT / 2 * w[2] * q.W,
                q.W - deltaT / 2 * w[0] * q.X - deltaT / 2 * w[1] * q.Y - deltaT / 2 * w[2] * q.Z
                );
            */
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
            //q = Quaternion.Normalize(q);
            Matrix<float> C = CreateFromQuaternionPy(q).Transpose();
            MathNet.Numerics.LinearAlgebra.Vector<float> a_hat = C * g;
            MathNet.Numerics.LinearAlgebra.Vector<float> m_hat = C * r;
            MathNet.Numerics.LinearAlgebra.Vector<float> result = CreateFromVectors(
                new MathNet.Numerics.LinearAlgebra.Vector<float>[] {a_hat, m_hat}
                );
            return result;
        }
        // Copia del repositorio de ekf de python, es diferente de la de system.numerics
        public static Matrix<float> CreateFromQuaternionPy(Quaternion q)
        {
            Matrix<float> result = Matrix<float>.Build.DenseOfArray(
                new float[,]
                {
                { 1 - 2 * (q.Y * q.Y + q.Z * q.Z), 2 * (q.X * q.Y - q.W * q.Z), 2 * (q.X * q.Z + q.W * q.Y) },
                { 2 * (q.X * q.Y + q.W * q.Z), 1 - 2 * (q.X * q.X + q.Z * q.Z), 2 * (q.Y * q.Z - q.W * q.X) },
                { 2 * (q.X * q.Z - q.W * q.Y), 2 * (q.W * q.X + q.Y * q.Z), 1 - 2 * (q.X * q.X + q.Y * q.Y) }
                }
                );

            return result;
        }
        public static Matrix<float> CreateFromQuaternion(Quaternion quaternion)
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
                q.W, q.X, q.Y, q.Z
            });
        }
        public static Quaternion ToQuaternion(MathNet.Numerics.LinearAlgebra.Vector<float> v)
        {
            return new Quaternion(v[1], v[2], v[3], v[0]);
        }
        private Matrix<float> buildH(Quaternion q)
        {
            //q = Quaternion.Normalize(q);
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
        private Matrix<float> buildR()
        {
            Matrix<float> M11 = Matrix<float>.Build.DenseDiagonal(3, var_acc);
            Matrix<float> M12 = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> M21 = Matrix<float>.Build.Dense(3, 3, 0);
            Matrix<float> M22 = Matrix<float>.Build.DenseDiagonal(3, var_mag);
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
                Q = W * W.Transpose() * (float)spectralNoise * deltaT / 2;
            }
            else
            {
                Q = W * spectralNoiseCovarianceMatrix * W.Transpose() * deltaT / 2;
            }
            Matrix<float> F = buildF(gyr);
            Matrix<float> P_hat = F * P * F.Transpose() + Q;
            Quaternion q_hat = buildqhat(q, gyr, deltaT);
            q_hat = Quaternion.Normalize(q_hat);
            #endregion prediction
            #region correction
            acc = acc.Normalize(2);
            mag = mag.Normalize(2);
            MathNet.Numerics.LinearAlgebra.Vector<float> z = CreateFromVectors(
                new MathNet.Numerics.LinearAlgebra.Vector<float>[]
                {
                    acc, mag
                });
            h = buildh(q_hat);
            MathNet.Numerics.LinearAlgebra.Vector<float> v = z - h;
            H = buildH(q_hat);
            Matrix<float> R = buildR();
            Matrix<float> S = H * P_hat * H.Transpose() + R;
            Matrix<float> K = P_hat * H.Transpose() * S.Inverse();
            MathNet.Numerics.LinearAlgebra.Vector<float> q_vector = ToVector(q_hat) + K * v;
            P = (Matrix<float>.Build.DenseIdentity(4) - K * H) * P_hat;
            q_vector = q_vector.Normalize(2); //2???
            #endregion correction
            return ToQuaternion(q_vector);
        }
        // Esto es para combrobar que funcione bien
        private Quaternion update_debug(Quaternion q, Matrix<float> P, MathNet.Numerics.LinearAlgebra.Vector<float> gyr,
            MathNet.Numerics.LinearAlgebra.Vector<float> acc, MathNet.Numerics.LinearAlgebra.Vector<float> mag)
        {
            #region prediction
            Matrix<float> W = buildW(q);
            Matrix<float> Q;
            if (spectralNoiseCovarianceMatrix == null)
            {
                Q = W * W.Transpose() * (float)spectralNoise * deltaT / 2;
            }
            else
            {
                Q = W * spectralNoiseCovarianceMatrix * W.Transpose() * deltaT / 2;
            }
            Trace.WriteLine("Q");
            Trace.WriteLine("expected:");
            Trace.WriteLine(@"
[[5.62489211e-09 0.00000000e+00 5.62489211e-09 0.00000000e+00]
 [0.00000000e+00 1.12497842e-08 0.00000000e+00 2.90943113e-25]
 [5.62489211e-09 0.00000000e+00 5.62489211e-09 0.00000000e+00]
 [0.00000000e+00 2.90943113e-25 0.00000000e+00 1.12497842e-08]]
");
            Trace.WriteLine("calculated:");
            Trace.WriteLine(Q);
            Matrix<float> F = buildF(gyr);
            Trace.WriteLine("F");
            Trace.WriteLine("expected:");
            Trace.WriteLine(@"
[[ 1.         -0.98275513 -2.7753714   3.78256226]
 [ 0.98275513  1.         -3.78256226 -2.7753714 ]
 [ 2.7753714   3.78256226  1.          0.98275513]
 [-3.78256226  2.7753714  -0.98275513  1.        ]]
");
            Trace.WriteLine("calculated:");
            Trace.WriteLine(F);
            Matrix<float> P_hat = F * P * F.Transpose() + Q;
            Trace.WriteLine("P_hat");
            Trace.WriteLine("expected:");
            Trace.WriteLine(@"
[[ 2.39762713e+01  4.35579399e-16  5.62489217e-09  0.00000000e+00]
 [ 4.35579399e-16  2.39762713e+01 -1.91929026e-16  2.90943113e-25]
 [ 5.62489217e-09 -1.91929026e-16  2.39762713e+01 -4.44089210e-16]
 [ 0.00000000e+00  2.90943113e-25 -4.44089210e-16  2.39762713e+01]]
");
            Trace.WriteLine("calculated:");
            Trace.WriteLine(P_hat);
            Quaternion q_hat = buildqhat(q, gyr, deltaT);
            Trace.WriteLine("q_hat");
            Trace.WriteLine("expected:");
            Trace.WriteLine("[ 2.66956512  3.36955592  1.25536512 -1.97974362]");
            Trace.WriteLine("calculated:");
            Trace.WriteLine(q_hat);
            Trace.WriteLine(ToVector(q_hat));
            q_hat = Quaternion.Normalize(q_hat);
            #endregion prediction
            #region correction
            acc = acc.Normalize(2);
            mag = mag.Normalize(2);
            MathNet.Numerics.LinearAlgebra.Vector<float> z = CreateFromVectors(
                new MathNet.Numerics.LinearAlgebra.Vector<float>[]
                {
                    acc, mag
                });
            Trace.WriteLine("z");
            Trace.WriteLine("expected:");
            Trace.WriteLine("[ 0.97546371 -0.18237299 -0.12333142 -0.23436276 -0.40269051  0.88482453]");
            Trace.WriteLine("calculated:");
            Trace.WriteLine(z);
            h = buildh(q_hat);
            Trace.WriteLine("h");
            Trace.WriteLine("expected:");
            Trace.WriteLine(@"
[ 0.83602057 - 0.54304297  0.07857444 - 0.45321799  0.86715055 - 0.20650275]
");
            Trace.WriteLine("calculated:");
            Trace.WriteLine(h);
            MathNet.Numerics.LinearAlgebra.Vector<float> v = z - h;
            Trace.WriteLine("v");
            Trace.WriteLine("expected:");
            Trace.WriteLine(@"
[ 0.13944314  0.36066998 -0.20190586  0.21885523 -1.26984106  1.09132728]
");
            Trace.WriteLine("calculated:");
            Trace.WriteLine(v);
            H = buildH(q_hat);
            Trace.WriteLine("H");
            Trace.WriteLine("expected:");
            Trace.WriteLine(@"
[[ 0.51275912  0.80863455  1.09039502 -1.37630918]
 [-1.37630918 -1.09039502  0.80863455 -0.51275912]
 [ 0.          2.75261836  1.02551824  0.        ]
 [-0.44406243 -0.70029806 -1.45706891  2.00055326]
 [ 1.59623599  1.20068934 -0.01214347 -0.10113508]
 [ 0.25637956 -2.7881547  -0.34292734  0.68815459]]
");
            Trace.WriteLine("calculated:");
            Trace.WriteLine(H);
            Matrix<float> R = buildR();
            Trace.WriteLine("R");
            Trace.WriteLine("expected:");
            Trace.WriteLine(@"
[[0.25 0.   0.   0.   0.   0.  ]
 [0.   0.25 0.   0.   0.   0.  ]
 [0.   0.   0.25 0.   0.   0.  ]
 [0.   0.   0.   0.64 0.   0.  ]
 [0.   0.   0.   0.   0.64 0.  ]
 [0.   0.   0.   0.   0.   0.64]]
");
            Trace.WriteLine("calculated:");
            Trace.WriteLine(R);
            Matrix<float> S = H * P_hat * H.Transpose() + R;
            Trace.WriteLine("S");
            Trace.WriteLine("expected:");
            Trace.WriteLine(@"
[[ 9.61550851e+01 -7.09918852e-09  8.01786240e+01 -1.23145552e+02
4.59230809e+01 -8.25785221e+01]
 [-7.09918622e-09  9.61550851e+01 -5.20805819e+01 -1.98827900e+01
  -8.30562400e+01  4.93233141e+01]
 [ 8.01786240e+01 -5.20805819e+01  2.07131547e+02 -8.20444996e+01
   7.89439397e+01 -1.92443242e+02]
 [-1.23145552e+02 -1.98827900e+01 -8.20444996e+01  1.63987266e+02
  -4.15820734e+01  8.90730341e+01]
 [ 4.59230809e+01 -8.30562400e+01  7.89439397e+01 -4.15820734e+01
   9.65450851e+01 -7.20222638e+01]
 [-8.25785221e+01  4.93233141e+01 -1.92443242e+02  8.90730341e+01
  -7.20222638e+01  2.02776583e+02]]
");
            Trace.WriteLine("calculated:");
            Trace.WriteLine(S);
            Matrix<float> K = P_hat * H.Transpose() * S.Inverse();
            Trace.WriteLine("K");
            Trace.WriteLine("expected:");
            Trace.WriteLine(@"
[[ 0.19405421 -0.2847888   0.06118142 -0.04829303  0.24579322  0.34519077]
 [-0.05456274 -0.11639976  0.09660976  0.01926712 -0.03537657 -0.25291987]
 [ 0.17385278  0.36786833  0.68768931 -0.00926857  0.14989112  0.65072671]
 [-0.22112065  0.08325712  0.60396859  0.19704567  0.11447141  0.49836008]]
");
            Trace.WriteLine("calculated:");
            Trace.WriteLine(K);
            MathNet.Numerics.LinearAlgebra.Vector<float> q_vector = ToVector(q_hat) + K * v;
            Trace.WriteLine("q");
            Trace.WriteLine("expected:");
            Trace.WriteLine("[ 0.51121799  0.39217919  0.79224213 -0.08542955]");
            Trace.WriteLine("calculated:");
            Trace.WriteLine(q_vector);
            P = (Matrix<float>.Build.DenseIdentity(4) - K * H) * P_hat;
            Trace.WriteLine("P");
            Trace.WriteLine("expected:");
            Trace.WriteLine(@"
[[ 0.14986459 -0.05648494  0.16652736  0.11933061]
 [-0.05648494  0.04936854 -0.10895984 -0.06845166]
 [ 0.16652736 -0.10895984  0.46010607  0.33096772]
 [ 0.11933061 -0.06845166  0.33096772  0.30661802]]
");
            Trace.WriteLine("calculated:");
            Trace.WriteLine(P);
            #endregion correction
            return Quaternion.Normalize(ToQuaternion(q_vector));
        }
        public static void test()
        {
            Quaternion q0 = new Quaternion(0.0f, -0.7071f, 0.0f, 0.7071f);
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
            void testh() // Funciona bien
            {
                EKF ekf = new EKF(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED: true);
                Trace.WriteLine("h");
                Trace.WriteLine("expected:");
                Trace.WriteLine("[-1.00000000e+00  0.00000000e+00  2.22044605e-16  8.66025404e-01 0.00000000e+00 -5.00000000e-01]");
                Trace.WriteLine("calculated:");
                Trace.WriteLine(ekf.buildh(q0));
            }
            void testH() // Funciona bien
            {
                EKF ekf = new EKF(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED: true);
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
                EKF ekf = new EKF(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED: true);
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
            void testFull() // Resultado incorrecto
            {
                Trace.WriteLine("start EKF test");
                EKF ekf = new EKF(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED:true);
                Matrix<float> P = Matrix<float>.Build.DenseIdentity(4);
                Quaternion q = ekf.update(q0, gyr, acc, mag);
                Trace.WriteLine("end EKF test");
                Trace.WriteLine("calculated:");
                Trace.WriteLine(q);
                Trace.WriteLine(ToVector(q));
                Trace.WriteLine("expected:");
                Trace.WriteLine("[ 0.4988753   0.38271055  0.77311449 -0.08336696]");
            }
            void testToVQConversion() // Funciona bien
            {
                EKF ekf = new EKF(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED: true);
                Trace.WriteLine("original quaternion: ");
                Trace.WriteLine(q0);
                MathNet.Numerics.LinearAlgebra.Vector<float> v = ToVector(q0);
                Trace.WriteLine("vector: ");
                Trace.WriteLine(v);
                Quaternion q = ToQuaternion(v);
                Trace.WriteLine("quaternion converted: ");
                Trace.WriteLine(q);
            }
            void testOmega() //Funciona bien
            {
                EKF ekf = new EKF(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED: true);
                Trace.WriteLine("Omega");
                Trace.WriteLine("expected:");
                Trace.WriteLine(@"
[[   0.         -196.55102539 -555.07427979  756.51245117]
 [ 196.55102539    0.         -756.51245117 -555.07427979]
 [ 555.07427979  756.51245117    0.          196.55102539]
 [-756.51245117  555.07427979 -196.55102539    0.        ]]
");
                Trace.WriteLine("calculated:");
                Trace.WriteLine(ekf.buildOmega(gyr));
            }
            void testqhat() // Funciona bien
            {
                EKF ekf = new EKF(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED: true);
                Trace.WriteLine("qhat");
                Trace.WriteLine("expected:");
                Trace.WriteLine("[3.36958824  1.25537716 -1.97976261 2.66959072]");
                Trace.WriteLine("calculated:");
                Trace.WriteLine(ekf.buildqhat(q0, gyr, 0.01f));
            }
            void testW() //Funciona bien
            {
                EKF ekf = new EKF(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED: true);
                Trace.WriteLine("W");
                Trace.WriteLine("expected:");
                Trace.WriteLine(@"
[[-0.          0.00353553 -0.        ]
 [ 0.00353553  0.         -0.00353553]
 [ 0.          0.00353553  0.        ]
 [ 0.00353553  0.          0.00353553]]
");
                Trace.WriteLine("calculated:");
                Trace.WriteLine(ekf.buildW(q0));
            }
            testFull();
            //testToVQConversion();
            //testh();
            //testH();
            //testF();
            //testOmega();
            //testqhat();
            //testW();
            //EKF ekf = new EKF(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED: true);
            //Matrix<float> P = Matrix<float>.Build.DenseIdentity(4);
            //ekf.update_debug(q0, P, gyr, acc, mag);
        }
    }
}
