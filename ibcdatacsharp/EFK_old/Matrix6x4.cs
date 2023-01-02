

using System;
using System.Diagnostics;
using System.Numerics;

namespace ibcdatacsharp.EFK_old
{
    public struct Matrix6x4
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
        /// Value at row 1, column 4 of the matrix.
        /// </summary>
        public float M14;

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
        /// Value at row 2, column 4 of the matrix.
        /// </summary>
        public float M24;

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
        /// Value at row 3, column 4 of the matrix.
        /// </summary>
        public float M34;

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
        /// <summary>
        /// Value at row 4, column 4 of the matrix.
        /// </summary>
        public float M44;

        /// <summary>
        /// Value at row 4, column 1 of the matrix.
        /// </summary>
        public float M51;
        /// <summary>
        /// Value at row 4, column 2 of the matrix.
        /// </summary>
        public float M52;
        /// <summary>
        /// Value at row 4, column 3 of the matrix.
        /// </summary>
        public float M53;
        /// <summary>
        /// Value at row 4, column 4 of the matrix.
        /// </summary>
        public float M54;

        /// <summary>
        /// Value at row 4, column 1 of the matrix.
        /// </summary>
        public float M61;
        /// <summary>
        /// Value at row 4, column 2 of the matrix.
        /// </summary>
        public float M62;
        /// <summary>
        /// Value at row 4, column 3 of the matrix.
        /// </summary>
        public float M63;
        /// <summary>
        /// Value at row 4, column 4 of the matrix.
        /// </summary>
        public float M64;
        #endregion Public Fields
        /// <summary>
        /// Constructs a Matrix4x4 from the given components.
        /// </summary>
        public Matrix6x4(float m11, float m12, float m13, float m14,
                         float m21, float m22, float m23, float m24,
                         float m31, float m32, float m33, float m34,
                         float m41, float m42, float m43, float m44,
                         float m51, float m52, float m53, float m54,
                         float m61, float m62, float m63, float m64)
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;

            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;

            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;

            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;

            this.M51 = m51;
            this.M52 = m52;
            this.M53 = m53;
            this.M54 = m54;

            this.M61 = m61;
            this.M62 = m62;
            this.M63 = m63;
            this.M64 = m64;
        }
        public static Matrix6x4 Transpose(Matrix4x6 matrix)
        {
            Matrix6x4 result = new Matrix6x4();

            result.M11 = matrix.M11;
            result.M21 = matrix.M12;
            result.M31 = matrix.M13;
            result.M41 = matrix.M14;
            result.M51 = matrix.M15;
            result.M61 = matrix.M16;

            result.M12 = matrix.M21;
            result.M22 = matrix.M22;
            result.M32 = matrix.M23;
            result.M42 = matrix.M24;
            result.M52 = matrix.M25;
            result.M62 = matrix.M26;

            result.M13 = matrix.M31;
            result.M23 = matrix.M32;
            result.M33 = matrix.M33;
            result.M43 = matrix.M34;
            result.M53 = matrix.M35;
            result.M63 = matrix.M36;

            result.M14 = matrix.M41;
            result.M24 = matrix.M42;
            result.M34 = matrix.M43;
            result.M44 = matrix.M44;
            result.M54 = matrix.M45;
            result.M64 = matrix.M46;

            return result;
        }
        public static Matrix6x4 Multiply(Matrix6x4 value1, float value2)
        {
            Matrix6x4 result;

            result.M11 = value1.M11 * value2;
            result.M12 = value1.M12 * value2;
            result.M13 = value1.M13 * value2;
            result.M14 = value1.M14 * value2;

            result.M21 = value1.M21 * value2;
            result.M22 = value1.M22 * value2;
            result.M23 = value1.M23 * value2;
            result.M24 = value1.M24 * value2;

            result.M31 = value1.M31 * value2;
            result.M32 = value1.M32 * value2;
            result.M33 = value1.M33 * value2;
            result.M34 = value1.M34 * value2;

            result.M41 = value1.M41 * value2;
            result.M42 = value1.M42 * value2;
            result.M43 = value1.M43 * value2;
            result.M44 = value1.M44 * value2;

            result.M51 = value1.M51 * value2;
            result.M52 = value1.M52 * value2;
            result.M53 = value1.M53 * value2;
            result.M54 = value1.M54 * value2;

            result.M61 = value1.M61 * value2;
            result.M62 = value1.M62 * value2;
            result.M63 = value1.M63 * value2;
            result.M64 = value1.M64 * value2;

            return result;
        }
        public static Matrix6x4 operator *(Matrix6x4 value1, float value2)
        {
            return Multiply(value1, value2);
        }
        public static Matrix6x4 operator *(float value1, Matrix6x4 value2)
        {
            return Multiply(value2, value1);
        }
        public static Matrix6x4 Multiply(Matrix6x4 value1, Matrix4x4 value2)
        {
            Matrix6x4 result;

            // First row
            result.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21 + value1.M13 * value2.M31 + value1.M14 * value2.M41;
            result.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22 + value1.M13 * value2.M32 + value1.M14 * value2.M42;
            result.M13 = value1.M11 * value2.M13 + value1.M12 * value2.M23 + value1.M13 * value2.M33 + value1.M14 * value2.M43;
            result.M14 = value1.M11 * value2.M14 + value1.M12 * value2.M24 + value1.M13 * value2.M34 + value1.M14 * value2.M44;

            // Second row
            result.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21 + value1.M23 * value2.M31 + value1.M24 * value2.M41;
            result.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22 + value1.M23 * value2.M32 + value1.M24 * value2.M42;
            result.M23 = value1.M21 * value2.M13 + value1.M22 * value2.M23 + value1.M23 * value2.M33 + value1.M24 * value2.M43;
            result.M24 = value1.M21 * value2.M14 + value1.M22 * value2.M24 + value1.M23 * value2.M34 + value1.M24 * value2.M44;

            // Third row
            result.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value1.M33 * value2.M31 + value1.M34 * value2.M41;
            result.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value1.M33 * value2.M32 + value1.M34 * value2.M42;
            result.M33 = value1.M31 * value2.M13 + value1.M32 * value2.M23 + value1.M33 * value2.M33 + value1.M34 * value2.M43;
            result.M34 = value1.M31 * value2.M14 + value1.M32 * value2.M24 + value1.M33 * value2.M34 + value1.M34 * value2.M44;

            // Fourth row
            result.M41 = value1.M41 * value2.M11 + value1.M42 * value2.M21 + value1.M43 * value2.M31 + value1.M44 * value2.M41;
            result.M42 = value1.M41 * value2.M12 + value1.M42 * value2.M22 + value1.M43 * value2.M32 + value1.M44 * value2.M42;
            result.M43 = value1.M41 * value2.M13 + value1.M42 * value2.M23 + value1.M43 * value2.M33 + value1.M44 * value2.M43;
            result.M44 = value1.M41 * value2.M14 + value1.M42 * value2.M24 + value1.M43 * value2.M34 + value1.M44 * value2.M44;

            // Fifth row
            result.M51 = value1.M51 * value2.M11 + value1.M52 * value2.M21 + value1.M53 * value2.M31 + value1.M54 * value2.M41;
            result.M52 = value1.M51 * value2.M12 + value1.M52 * value2.M22 + value1.M53 * value2.M32 + value1.M54 * value2.M42;
            result.M53 = value1.M51 * value2.M13 + value1.M52 * value2.M23 + value1.M53 * value2.M33 + value1.M54 * value2.M43;
            result.M54 = value1.M51 * value2.M14 + value1.M52 * value2.M24 + value1.M53 * value2.M34 + value1.M54 * value2.M44;

            // Sixth row
            result.M61 = value1.M61 * value2.M11 + value1.M62 * value2.M21 + value1.M63 * value2.M31 + value1.M64 * value2.M41;
            result.M62 = value1.M61 * value2.M12 + value1.M62 * value2.M22 + value1.M63 * value2.M32 + value1.M64 * value2.M42;
            result.M63 = value1.M61 * value2.M13 + value1.M62 * value2.M23 + value1.M63 * value2.M33 + value1.M64 * value2.M43;
            result.M64 = value1.M61 * value2.M14 + value1.M62 * value2.M24 + value1.M63 * value2.M34 + value1.M64 * value2.M44;

            return result;
        }
        public static Matrix6x4 operator *(Matrix6x4 value1, Matrix4x4 value2)
        {
            return Multiply(value1, value2);
        }
        public static Matrix6x6 Multiply(Matrix6x4 value1, Matrix4x6 value2)
        {
            Matrix6x6 m;

            // First row
            m.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21 + value1.M13 * value2.M31 + value1.M14 * value2.M41;
            m.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22 + value1.M13 * value2.M32 + value1.M14 * value2.M42;
            m.M13 = value1.M11 * value2.M13 + value1.M12 * value2.M23 + value1.M13 * value2.M33 + value1.M14 * value2.M43;
            m.M14 = value1.M11 * value2.M14 + value1.M12 * value2.M24 + value1.M13 * value2.M34 + value1.M14 * value2.M44;
            m.M15 = value1.M11 * value2.M15 + value1.M12 * value2.M25 + value1.M13 * value2.M35 + value1.M14 * value2.M45;
            m.M16 = value1.M11 * value2.M16 + value1.M12 * value2.M26 + value1.M13 * value2.M36 + value1.M14 * value2.M46;

            // Second row
            m.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21 + value1.M23 * value2.M31 + value1.M24 * value2.M41;
            m.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22 + value1.M23 * value2.M32 + value1.M24 * value2.M42;
            m.M23 = value1.M21 * value2.M13 + value1.M22 * value2.M23 + value1.M23 * value2.M33 + value1.M24 * value2.M43;
            m.M24 = value1.M21 * value2.M14 + value1.M22 * value2.M24 + value1.M23 * value2.M34 + value1.M24 * value2.M44;
            m.M25 = value1.M21 * value2.M15 + value1.M22 * value2.M25 + value1.M23 * value2.M35 + value1.M24 * value2.M45;
            m.M26 = value1.M21 * value2.M16 + value1.M22 * value2.M26 + value1.M23 * value2.M36 + value1.M24 * value2.M46;

            // Third row
            m.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value1.M33 * value2.M31 + value1.M34 * value2.M41;
            m.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value1.M33 * value2.M32 + value1.M34 * value2.M42;
            m.M33 = value1.M31 * value2.M13 + value1.M32 * value2.M23 + value1.M33 * value2.M33 + value1.M34 * value2.M43;
            m.M34 = value1.M31 * value2.M14 + value1.M32 * value2.M24 + value1.M33 * value2.M34 + value1.M34 * value2.M44;
            m.M35 = value1.M31 * value2.M15 + value1.M32 * value2.M25 + value1.M33 * value2.M35 + value1.M34 * value2.M45;
            m.M36 = value1.M31 * value2.M16 + value1.M32 * value2.M26 + value1.M33 * value2.M36 + value1.M34 * value2.M46;


            // Fourth row
            m.M41 = value1.M41 * value2.M11 + value1.M42 * value2.M21 + value1.M43 * value2.M31 + value1.M44 * value2.M41;
            m.M42 = value1.M41 * value2.M12 + value1.M42 * value2.M22 + value1.M43 * value2.M32 + value1.M44 * value2.M42;
            m.M43 = value1.M41 * value2.M13 + value1.M42 * value2.M23 + value1.M43 * value2.M33 + value1.M44 * value2.M43;
            m.M44 = value1.M41 * value2.M14 + value1.M42 * value2.M24 + value1.M43 * value2.M34 + value1.M44 * value2.M44;
            m.M45 = value1.M41 * value2.M15 + value1.M42 * value2.M25 + value1.M43 * value2.M35 + value1.M44 * value2.M45;
            m.M46 = value1.M41 * value2.M16 + value1.M42 * value2.M26 + value1.M43 * value2.M36 + value1.M44 * value2.M46;

            // Fifth row
            m.M51 = value1.M51 * value2.M11 + value1.M52 * value2.M21 + value1.M53 * value2.M31 + value1.M54 * value2.M41;
            m.M52 = value1.M51 * value2.M12 + value1.M52 * value2.M22 + value1.M53 * value2.M32 + value1.M54 * value2.M42;
            m.M53 = value1.M51 * value2.M13 + value1.M52 * value2.M23 + value1.M53 * value2.M33 + value1.M54 * value2.M43;
            m.M54 = value1.M51 * value2.M14 + value1.M52 * value2.M24 + value1.M53 * value2.M34 + value1.M54 * value2.M44;
            m.M55 = value1.M51 * value2.M15 + value1.M52 * value2.M25 + value1.M53 * value2.M35 + value1.M54 * value2.M45;
            m.M56 = value1.M51 * value2.M16 + value1.M52 * value2.M26 + value1.M53 * value2.M36 + value1.M54 * value2.M46;

            // Sixth row
            m.M61 = value1.M61 * value2.M11 + value1.M62 * value2.M21 + value1.M63 * value2.M31 + value1.M64 * value2.M41;
            m.M62 = value1.M61 * value2.M12 + value1.M62 * value2.M22 + value1.M63 * value2.M32 + value1.M64 * value2.M42;
            m.M63 = value1.M61 * value2.M13 + value1.M62 * value2.M23 + value1.M63 * value2.M33 + value1.M64 * value2.M43;
            m.M64 = value1.M61 * value2.M14 + value1.M62 * value2.M24 + value1.M63 * value2.M34 + value1.M64 * value2.M44;
            m.M65 = value1.M61 * value2.M15 + value1.M62 * value2.M25 + value1.M63 * value2.M35 + value1.M64 * value2.M45;
            m.M66 = value1.M61 * value2.M16 + value1.M62 * value2.M26 + value1.M63 * value2.M36 + value1.M64 * value2.M46;

            return m;
        }
        public static Matrix6x6 operator *(Matrix6x4 value1, Matrix4x6 value2)
        {
            return Multiply(value1, value2);
        }
        public bool Equals(Matrix6x4 other)
        {
            return (M11 == other.M11 && M22 == other.M22 && M33 == other.M33 && M44 == other.M44 && // Check diagonal element first for early out.
                                        M12 == other.M12 && M13 == other.M13 && M14 == other.M14 &&
                    M21 == other.M21 && M23 == other.M23 && M24 == other.M24 &&
                    M31 == other.M31 && M32 == other.M32 && M34 == other.M34 &&
                    M41 == other.M41 && M42 == other.M42 && M43 == other.M43 &&
                    M51 == other.M51 && M52 == other.M52 && M53 == other.M53 && M54 == other.M54 &&
                    M61 == other.M61 && M62 == other.M62 && M63 == other.M63 && M64 == other.M64);
        }
        public override bool Equals(object obj)
        {
            if (obj is Matrix6x4)
            {
                return Equals((Matrix6x4)obj);
            }

            return false;
        }
        public static void test()
        {
            void test1()
            {
                Matrix6x4 matrix6X4 = new Matrix6x4(
                    1, 2, -4, 1,
                    3, 5, -1, 2,
                    0, -3, 2, 5,
                    1, 7, 6, 9,
                    3, 5, -1, 2,
                    0, -3, 2, 5
                    );
                Matrix4x4 matrix4X4 = new Matrix4x4(
                    -9, -18, 7, 1,
                    -4, 19, 10, 5,
                    7, 2, -2, 6,
                    6, 72, -3, -9
                    );
                Matrix6x4 expected = new Matrix6x4(
                    - 39, 84,  32, - 22,
                    - 42, 183, 67,  4,
                    56,  307, - 49, - 48,
                    59,  775, 38, - 9,
                    - 42, 183, 67,  4,
                    56,  307, - 49, - 48
                    );
                Matrix6x4 result = Multiply(matrix6X4, matrix4X4);
                if (!result.Equals(expected))
                {
                    throw new System.Exception("matrix6x4 matrix4x4 multiply error");
                }
                else
                {
                    Trace.WriteLine("matrix6x4 matrix4x4 multiply correct");
                    Console.WriteLine("matrix6x4 matrix4x4 multiply correct");
                }
            }
            void test2()
            {
                Matrix6x4 matrix6X4 = new Matrix6x4(
                    1, 2, -4, 1,
                    3, 5, -1, 2,
                    0, -3, 2, 5,
                    1, 7, 6, 9,
                    3, 5, -1, 2,
                    0, -3, 2, 5
                    );
                Matrix4x6 matrix4X6 = new Matrix4x6(
                    1, 2, 3, 5, 1, 0,
                    -1, 4, 0, -2, 5, 3,
                    2, 7, -1, 0, -1, 4,
                    1, 2, 3, 5, 1, 0
                    );
                Matrix6x6 expected = new Matrix6x6(
                    - 8, - 16, 10,  6,   16, - 10,
                    - 2,  23, 16,  15,  31,  11,
                    12,  12,  13,  31, - 12, - 1,
                    15,  90,  24,  36,  39,  45,
                    - 2,  23,  16,  15,  31,  11,
                    12,  12,  13,  31, - 12, - 1
                    );
                Matrix6x6 result = Multiply(matrix6X4, matrix4X6);
                if (!result.Equals(expected))
                {
                    throw new System.Exception("matrix6x4 matrix4x6 multiply error");
                }
                else
                {
                    Trace.WriteLine("matrix6x4 matrix4x6 multiply correct");
                    Console.WriteLine("matrix6x4 matrix4x6 multiply correct");
                }
            }
            void test3()
            {
                Matrix6x4 matrix1 = new Matrix6x4(
                    1, 2, -4, 1,
                    3, 5, -1, 2,
                    0, -3, 2, 5,
                    1, 7, 6, 9,
                    3, 5, -1, 2,
                    0, -3, 2, 5
                    );
                Matrix6x4 matrix2 = new Matrix6x4(
                    1, 2, -4, 1,
                    3, 5, -1, 2,
                    0, -3, 2, 5,
                    1, 7, 6, 9,
                    3, 5, -1, 2,
                    0, -3, 2, 5
                    );
                Matrix6x4 matrix3 = new Matrix6x4(
                    1, 2, -3, 1,
                    3, 5, -1, 2,
                    0, -3, 2, 5,
                    1, 7, 6, 9,
                    3, 5, -1, 2,
                    0, -3, 2, 5
                    );
                if (matrix1.Equals(matrix2) && matrix2.Equals(matrix1) &&
                    !matrix1.Equals(matrix3) && !matrix3.Equals(matrix2))
                {
                    Trace.WriteLine("matrix6x4 equals correct");
                }
                else
                {
                    throw new System.Exception("matrix6x4 equals error");
                }
            }
            test1();
            test2();
            test3();
        }
    }
}
