//#define struct
#define class
using System;
using System.Diagnostics;
using System.Linq;

namespace ibcdatacsharp
{
#if struct
    public struct Quaternion
    {
        public Vector v;
        public float w;
        public Quaternion(float w, float x, float y, float z)
        {
            this.v = new Vector(x, y, z);
            this.w = w;
        }
        public Quaternion(Vector v, float w)
        {
            this.v = v;
            this.w = w;
        }
        public Quaternion(Vector v)
        {
            this.v = v;
            this.w = 0;
        }
        public static bool operator ==(Quaternion q1, Quaternion q2) => q1.v == q2.v && q1.w == q2.w;
        public static bool operator !=(Quaternion q1, Quaternion q2) => q1.v != q2.v || q1.w != q2.w;
        public override string ToString() => w.ToString() + " + " + v.ToString();
    }
    public struct Vector
    {
        public float x;
        public float y;
        public float z;
        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static Vector operator /(Vector v, float f) => new Vector(v.x / f, v.y / f, v.z / f);
        public static Vector operator -(Vector v) => new Vector(-v.x, -v.y, -v.z);
        public static Vector operator -(Vector v1, Vector v2) => new Vector(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        public static Vector operator +(Vector v1, Vector v2) => new Vector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        public static Vector operator *(float f, Vector v) => new Vector(f * v.x, f * v.y, f * v.z);
        public static bool operator ==(Vector v1, Vector v2) => v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        public static bool operator !=(Vector v1, Vector v2) => v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
        public override string ToString() => x.ToString() + " + " +  y.ToString() + " + " +  z.ToString();
    }
    public class LinearAcceleration
    {
        private Vector cross(Vector v1, Vector v2)
        {
            return new Vector(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
        }
        private float dot(Vector v1, Vector v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }
        private float dot(Quaternion q1, Quaternion q2)
        {
            return dot(q1.v, q2.v) + q1.w * q2.w;
        }
        private Quaternion normalizeQuaternion(Quaternion q)
        {
            float m = (float)Math.Sqrt(dot(q, q));
            return new Quaternion(q.v/m, q.w/m);
        }
        private Quaternion quaternionMult(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(cross(q1.v, q2.v) + q1.w * q2.v + q2.w * q1.v, q1.w * q2.w - dot(q1.v, q2.v));
        }
        private Quaternion quaternionConjugate(Quaternion q)
        {
            return new Quaternion(-q.v, q.w);
        }
        private Vector quaternionRotateVector(Quaternion q, Vector v)
        {
            Quaternion qv = quaternionMult(quaternionConjugate(q), new Quaternion(v));
            Quaternion qvq = quaternionMult(qv, q);
            return qvq.v;
        }
        private Vector vectorSubtraction(Vector v1, Vector v2)
        {
            return v1 - v2;
        }
        public Vector calcLinAcc(Quaternion q, Vector acc)
        {
            Vector g = new(0 , 0, -1);
            Vector gRot = quaternionRotateVector(q, g);
            return gRot - acc;
        }
#endif
#if class
    public class Quaternion
    {
        public Vector v;
        public float w;
        public Quaternion(float w, float x, float y, float z)
        {
            this.v = new Vector(x, y, z);
            this.w = w;
        }
        public Quaternion(Vector v, float w)
        {
            this.v = v;
            this.w = w;
        }
        public Quaternion(Vector v)
        {
            this.v = v;
            this.w = 0;
        }
        public static bool operator ==(Quaternion q1, Quaternion q2) => q1.v == q2.v && q1.w == q2.w;
        public static bool operator !=(Quaternion q1, Quaternion q2) => q1.v != q2.v || q1.w != q2.w;
        public override string ToString() => w.ToString() + " + " + v.ToString();
    }
    public class Vector
    {
        public float x;
        public float y;
        public float z;
        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static Vector operator /(Vector v, float f) => new Vector(v.x / f, v.y / f, v.z / f);
        public static Vector operator -(Vector v) => new Vector(-v.x, -v.y, -v.z);
        public static Vector operator -(Vector v1, Vector v2) => new Vector(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        public static Vector operator +(Vector v1, Vector v2) => new Vector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        public static Vector operator *(float f, Vector v) => new Vector(f * v.x, f * v.y, f * v.z);
        public static bool operator ==(Vector v1, Vector v2) => v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        public static bool operator !=(Vector v1, Vector v2) => v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
        public override string ToString() => x.ToString() + " + " +  y.ToString() + " + " +  z.ToString();
    }
    public class LinearAcceleration
    {
        private Vector cross(Vector v1, Vector v2)
        {
            return new Vector(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
        }
        private float dot(Vector v1, Vector v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }
        private float dot(Quaternion q1, Quaternion q2)
        {
            return dot(q1.v, q2.v) + q1.w * q2.w;
        }
        private Quaternion normalizeQuaternion(Quaternion q)
        {
            float m = (float)Math.Sqrt(dot(q, q));
            return new Quaternion(q.v/m, q.w/m);
        }
        private Quaternion quaternionMult(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(cross(q1.v, q2.v) + q1.w * q2.v + q2.w * q1.v, q1.w * q2.w - dot(q1.v, q2.v));
        }
        private Quaternion quaternionConjugate(Quaternion q)
        {
            return new Quaternion(-q.v, q.w);
        }
        private Vector quaternionRotateVector(Quaternion q, Vector v)
        {
            Quaternion qv = quaternionMult(quaternionConjugate(q), new Quaternion(v));
            Quaternion qvq = quaternionMult(qv, q);
            return qvq.v;
        }
        private Vector vectorSubtraction(Vector v1, Vector v2)
        {
            return v1 - v2;
        }
        public Vector calcLinAcc(Quaternion q, Vector acc)
        {
            Vector g = new(0 , 0, -1);
            Vector gRot = quaternionRotateVector(q, g);
            return gRot - acc;
        }
#endif
        private const int NUM_TESTS = 10;
        private const int NUM_OPERATIONS = 1000000;
        public void testQuaternionMult()
        {
            Quaternion q1 = new Quaternion(1.3f, 3.5f, 4.3f, 2.1f);
            Quaternion q2 = new Quaternion(2.4f, 5.7f, 8.1f, 6.8f);
            Quaternion r = quaternionMult(q1, q2);
            Quaternion tr = new Quaternion(-65.94f, 28.039999999999996f, 9.019999999999998f, 17.72f);
            Trace.WriteLine("teoric " + tr);
            Trace.WriteLine("calculated " + r);
        }
        public void testSpeed()
        {
            Vector getRandomVector(Random random)
            {
                return new Vector(random.NextSingle(), random.NextSingle(), random.NextSingle());
            }
            Quaternion getRandomQuaternion(Random random)
            {
                return new Quaternion(getRandomVector(random), random.NextSingle());
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Random random = new Random();
            float[] times = new float[NUM_TESTS];
            for (int j = 0; j < NUM_TESTS; j++)
            {
                for (int i = 0; i < NUM_OPERATIONS; i++)
                {
                    Quaternion q = getRandomQuaternion(random);
                    Vector v = getRandomVector(random);
                    Vector r = calcLinAcc(q, v);
                }
                times[j] = stopwatch.ElapsedMilliseconds;
                stopwatch.Restart();
            }
            Trace.Write('[');
            foreach(float time in times)
            {
                Trace.Write(time);
                Trace.Write(", ");
            }
            Trace.WriteLine("]");
            Trace.WriteLine(times.Sum() / times.Length);
        }
    }
}
