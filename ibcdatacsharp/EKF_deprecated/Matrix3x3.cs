

using System.Diagnostics;
using System.Numerics;

namespace ibcdatacsharp.EFK_old
{
    public struct Matrix3x3
    {
        #region Public Fields
        /// <summary>
        /// Value at row 1, column 1 of the matrix.
        /// </summary>
        public float M11;
        /// <summary>
        /// Value at row 1, column 2 of the matrix.
        /// </summary>
        public float M12;
        /// <summary>
        /// Value at row 1, column 3 of the matrix.
        /// </summary>
        public float M13;

        /// <summary>
        /// Value at row 2, column 1 of the matrix.
        /// </summary>
        public float M21;
        /// <summary>
        /// Value at row 2, column 2 of the matrix.
        /// </summary>
        public float M22;
        /// <summary>
        /// Value at row 2, column 3 of the matrix.
        /// </summary>
        public float M23;

        /// <summary>
        /// Value at row 3, column 1 of the matrix.
        /// </summary>
        public float M31;
        /// <summary>
        /// Value at row 3, column 2 of the matrix.
        /// </summary>
        public float M32;
        /// <summary>
        /// Value at row 3, column 3 of the matrix.
        /// </summary>
        public float M33;
        #endregion Public Fields
        public Matrix3x3(
            float m11, float m12, float m13,
            float m21, float m22, float m23,
            float m31, float m32, float m33
            )
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;

            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;

            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
        }
        public static Matrix3x3 CreateFromQuaternion(Quaternion quaternion)
        {
            Matrix4x4 m = Matrix4x4.CreateFromQuaternion(quaternion);

            Matrix3x3 result = new Matrix3x3(
                m.M11, m.M12, m.M13,
                m.M21, m.M22, m.M23,
                m.M31, m.M32, m.M33
                );

            return result;
        }
        public static Matrix3x3 Identity()
        {
            Matrix3x3 result = new Matrix3x3(
                1, 0, 0,
                0, 1, 0,
                0, 0, 1
                );
            return result;
        }
        public static Matrix3x3 Zero()
        {
            Matrix3x3 result = new Matrix3x3(
                0, 0, 0,
                0, 0, 0,
                0, 0, 0
                );
            return result;
        }
        public static Vector3 Multiply(Matrix3x3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.M11 * value2.X + value1.M12 * value2.Y + value1.M13 * value2.Z;
            result.Y = value1.M21 * value2.X + value1.M22 * value2.Y + value1.M23 * value2.Z;
            result.Z = value1.M31 * value2.X + value1.M32 * value2.Y + value1.M33 * value2.Z;

            return result;
        }
        public static Vector3 operator *(Matrix3x3 value1, Vector3 value2)
        {
            return Multiply(value1, value2);
        }
        public static Matrix3x3 Multiply(Matrix3x3 value1, float value2)
        {
            Matrix3x3 result;
            result.M11 = value1.M11 * value2;
            result.M12 = value1.M12 * value2;
            result.M13 = value1.M13 * value2;
            result.M21 = value1.M21 * value2;
            result.M22 = value1.M22 * value2;
            result.M23 = value1.M23 * value2;
            result.M31 = value1.M31 * value2;
            result.M32 = value1.M32 * value2;
            result.M33 = value1.M33 * value2;

            return result;
        }
        public static Matrix3x3 operator *(Matrix3x3 value1, float value2)
        {
            return Multiply(value1, value2);
        }
        public static Matrix3x3 operator *(float value1, Matrix3x3 value2)
        {
            return Multiply(value2, value1);
        }
        public static void test()
        {
            Matrix3x3 matrix = new Matrix3x3(
                2, 3, -4,
                11, 8, 7,
                2, 5, 3
                );
            Vector3 vector = new Vector3(3, 7, 5);
            Vector3 result = matrix * vector;
            Vector3 expected = new Vector3(7, 124, 56);
            if(result != expected)
            {
                throw new System.Exception("matrix3x3 vector3 multiply error");
            }
            else
            {
                Trace.WriteLine("matrix3x3 vector3 multiply correct");
            }
        }
    }
}
