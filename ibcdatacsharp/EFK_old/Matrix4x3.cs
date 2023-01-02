using System;
using System.Diagnostics;
using System.Numerics;

namespace ibcdatacsharp.EFK_old
{
    public struct Matrix4x3
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

        /// <summary>
        /// Value at row 4, column 1 of the matrix.
        /// </summary>
        public float M41;
        /// <summary>
        /// Value at row 4, column 2 of the matrix.
        /// </summary>
        public float M42;
        /// <summary>
        /// Value at row 4, column 3 of the matrix.
        /// </summary>
        public float M43;
        #endregion Public Fields
        public Matrix4x3(
            float m11, float m12, float m13,
            float m21, float m22, float m23,
            float m31, float m32, float m33,
            float m41, float m42, float m43
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

            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
        }
        public static Matrix4x3 Transpose(Matrix3x4 matrix)
        {
            Matrix4x3 result;

            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
            result.M41 = matrix.M14;
            result.M42 = matrix.M24;
            result.M43 = matrix.M34;

            return result;
        }
        public static Matrix4x4 Multiply(Matrix4x3 value1, Matrix3x4 value2)
        {
            Matrix4x4 result;

            // First row
            result.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21 + value1.M13 * value2.M31;
            result.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22 + value1.M13 * value2.M32;
            result.M13 = value1.M11 * value2.M13 + value1.M12 * value2.M23 + value1.M13 * value2.M33;
            result.M14 = value1.M11 * value2.M14 + value1.M12 * value2.M24 + value1.M13 * value2.M34;

            // Second row
            result.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21 + value1.M23 * value2.M31;
            result.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22 + value1.M23 * value2.M32;
            result.M23 = value1.M21 * value2.M13 + value1.M22 * value2.M23 + value1.M23 * value2.M33;
            result.M24 = value1.M21 * value2.M14 + value1.M22 * value2.M24 + value1.M23 * value2.M34;

            // Third row
            result.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value1.M33 * value2.M31;
            result.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value1.M33 * value2.M32;
            result.M33 = value1.M31 * value2.M13 + value1.M32 * value2.M23 + value1.M33 * value2.M33;
            result.M34 = value1.M31 * value2.M14 + value1.M32 * value2.M24 + value1.M33 * value2.M34;

            // Fourth row
            result.M41 = value1.M41 * value2.M11 + value1.M42 * value2.M21 + value1.M43 * value2.M31;
            result.M42 = value1.M41 * value2.M12 + value1.M42 * value2.M22 + value1.M43 * value2.M32;
            result.M43 = value1.M41 * value2.M13 + value1.M42 * value2.M23 + value1.M43 * value2.M33;
            result.M44 = value1.M41 * value2.M14 + value1.M42 * value2.M24 + value1.M43 * value2.M34;

            return result;
        }
        public static Matrix4x4 operator *(Matrix4x3 value1, Matrix3x4 value2)
        {
            return Multiply(value1, value2);
        }
        public static Matrix4x3 Multiply(Matrix4x3 value1, float value2)
        {
            Matrix4x3 result;

            result.M11 = value1.M11 * value2;
            result.M12 = value1.M12 * value2;
            result.M13 = value1.M13 * value2;
            result.M21 = value1.M21 * value2;
            result.M22 = value1.M22 * value2;
            result.M23 = value1.M23 * value2;
            result.M31 = value1.M31 * value2;
            result.M32 = value1.M32 * value2;
            result.M33 = value1.M33 * value2;
            result.M41 = value1.M41 * value2;
            result.M42 = value1.M42 * value2;
            result.M43 = value1.M43 * value2;

            return result;
        }
        public static Matrix4x3 operator *(Matrix4x3 value1, float value2)
        {
            return Multiply(value1, value2);
        }
        public static Matrix4x3 operator *(float value1, Matrix4x3 value2)
        {
            return Multiply(value2, value1);
        }
        public static Matrix4x3 Multiply(Matrix4x3 value1, Matrix3x3 value2) //Posible error
        {
            Matrix4x3 result;

            // First row
            result.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21 + value1.M13 * value2.M31;
            result.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22 + value1.M13 * value2.M32;
            result.M13 = value1.M11 * value2.M13 + value1.M12 * value2.M23 + value1.M13 * value2.M33;

            // Second row
            result.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21 + value1.M23 * value2.M31;
            result.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22 + value1.M23 * value2.M32;
            result.M23 = value1.M21 * value2.M13 + value1.M22 * value2.M23 + value1.M23 * value2.M33;

            // Third row
            result.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value1.M33 * value2.M31;
            result.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value1.M33 * value2.M32;
            result.M33 = value1.M31 * value2.M13 + value1.M32 * value2.M23 + value1.M33 * value2.M33;

            // Fourth row
            result.M41 = value1.M41 * value2.M11 + value1.M42 * value2.M21 + value1.M43 * value2.M31;
            result.M42 = value1.M41 * value2.M12 + value1.M42 * value2.M22 + value1.M43 * value2.M32;
            result.M43 = value1.M41 * value2.M13 + value1.M42 * value2.M23 + value1.M43 * value2.M33;

            return result;
        }
        public static Matrix4x3 operator *(Matrix4x3 value1, Matrix3x3 value2)
        {
            return Multiply(value1, value2);
        }
        public bool Equals(Matrix4x3 other)
        {
            return (M11 == other.M11 && M22 == other.M22 && M33 == other.M33 &&  // Check diagonal element first for early out.
                                        M12 == other.M12 && M13 == other.M13 && 
                    M21 == other.M21 && M23 == other.M23 &&
                    M31 == other.M31 && M32 == other.M32 &&
                    M41 == other.M41 && M42 == other.M42 && M43 == other.M43);
        }
        public override bool Equals(object obj)
        {
            if (obj is Matrix4x3)
            {
                return Equals((Matrix4x3)obj);
            }

            return false;
        }
        public static void test()
        {
            void test1()
            {
                Matrix4x3 matrix4X3 = new Matrix4x3(
                    1, 2, -4,
                    3, 5, -1,
                    0, -3, 2,
                    1, 7, 6
                    );
                Matrix3x3 matrix3X3 = new Matrix3x3(
                    1, 2, 3,
                    -1, 4, 0,
                    2, 7, -1
                    );
                Matrix4x3 expected = new Matrix4x3(
                    -9, -18, 7,
                    -4, 19, 10,
                    7, 2, -2,
                    6, 72, -3
                    );
                Matrix4x3 result = Multiply(matrix4X3, matrix3X3);
                if (!result.Equals(expected))
                {
                    throw new System.Exception("matrix4x3 matrix3x3 multiply error");
                }
                else
                {
                    Trace.WriteLine("matrix4x3 matrix3x3 multiply correct");
                    Console.WriteLine("matrix4x3 matrix3x3 multiply correct");
                }
            }
            void test2()
            {
                Matrix4x3 matrix4X3 = new Matrix4x3(
                    1, 2, -4,
                    3, 5, -1,
                    0, -3, 2,
                    1, 7, 6
                    );
                Matrix3x4 matrix3X4 = new Matrix3x4(
                    1, 2, 3, 5,
                    -1, 4, 0, -2,
                    2, 7, -1, 0
                    );
                Matrix4x4 expected = new Matrix4x4(
                    - 9, - 18, 7,   1,
                    - 4,  19,  10,  5,
                    7,   2, - 2,  6,
                    6,   72, - 3, - 9
                    );
                Matrix4x4 result = Multiply(matrix4X3, matrix3X4);
                if (!result.Equals(expected))
                {
                    throw new System.Exception("matrix4x3 matrix3x4 multiply error");
                }
                else
                {
                    Trace.WriteLine("matrix4x3 matrix3x4 multiply correct");
                    Console.WriteLine("matrix4x3 matrix3x4 multiply correct");
                }
            }
            void test3()
            {
                Matrix4x3 matrix1 = new Matrix4x3(
                    1, 2, -4,
                    3, 5, -1,
                    0, -3, 2,
                    1, 7, 6
                    );
                Matrix4x3 matrix2 = new Matrix4x3(
                    1, 2, -4,
                    3, 5, -1,
                    0, -3, 2,
                    1, 7, 6
                    );
                Matrix4x3 matrix3 = new Matrix4x3(
                    1, 2, -4,
                    3, 5, -1,
                    0, -3, 2,
                    1, 7, 5
                    );
                if(matrix1.Equals(matrix2) && matrix2.Equals(matrix1) && 
                    !matrix1.Equals(matrix3) && !matrix3.Equals(matrix2))
                {
                    Trace.WriteLine("matrix4x3 equals correct");
                }
                else
                {
                    throw new System.Exception("matrix4x3 equals error");
                }
            }
            test1();
            test2();
            test3();
        }
    }
}
