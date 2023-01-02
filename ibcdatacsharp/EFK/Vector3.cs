using MathNet.Numerics.LinearAlgebra;

namespace ibcdatacsharp.EFK
{
    public struct Vector3
    {
        private Vector<float> data;
        public Vector3(Vector<float> data)
        {
            this.data = data;
        }
        public Vector3(float x, float y, float z)
        {
            data = Vector<float>.Build.Dense(
                new float[] { x, y, z }
                ); ;
        }
        public float x
        {
            get
            {
                return data[0];
            }
            set
            {
                data[0] = value;
            }
        }
        public float y
        {
            get
            {
                return data[1];
            }
            set
            {
                data[1] = value;
            }
        }
        public float z
        {
            get
            {
                return data[2];
            }
            set
            {
                data[2] = value;
            }
        }
    }
}
