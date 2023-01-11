

using System.Diagnostics;
using System;
using System.Numerics;

namespace ibcdatacsharp.EFK_old
{
    public struct Matrix4x6
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
        /// Value at row 1, column 5 of the matrix.
        /// </summary>
        public float M15;
        /// <summary>
        /// Value at row 1, column 6 of the matrix.
        /// </summary>
        public float M16;

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
        /// Value at row 2, column 5 of the matrix.
        /// </summary>
        public float M25;
        /// <summary>
        /// Value at row 2, column 6 of the matrix.
        /// </summary>
        public float M26;

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
        /// Value at row 3, column 5 of the matrix.
        /// </summary>
        public float M35;
        /// <summary>
        /// Value at row 3, column 6 of the matrix.
        /// </summary>
        public float M36;

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
        /// Value at row 4, column 5 of the matrix.
        /// </summary>
        public float M45;
        /// <summary>
        /// Value at row 4, column 6 of the matrix.
        /// </summary>
        public float M46;
        #endregion Public Fields
        /// <summary>
        /// Constructs a Matrix4x4 from the given components.
        /// </summary>
        public Matrix4x6(float m11, float m12, float m13, float m14, float m15, float m16,
                         float m21, float m22, float m23, float m24, float m25, float m26,
                         float m31, float m32, float m33, float m34, float m35, float m36,
                         float m41, float m42, float m43, float m44, float m45, float m46
                         )
        {
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M14 = m14;
            this.M15 = m15;
            this.M16 = m16;

            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M24 = m24;
            this.M25 = m25;
            this.M26 = m26;

            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
            this.M34 = m34;
            this.M35 = m35;
            this.M36 = m36;

            this.M41 = m41;
            this.M42 = m42;
            this.M43 = m43;
            this.M44 = m44;
            this.M45 = m45;
            this.M46 = m46;
        }
        public static Matrix4x6 Transpose(Matrix6x4 matrix)
        {
            Matrix4x6 result = new Matrix4x6();

            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M14 = matrix.M41;
            result.M15 = matrix.M51;
            result.M16 = matrix.M61;

            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M24 = matrix.M42;
            result.M25 = matrix.M52;
            result.M26 = matrix.M62;

            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
            result.M34 = matrix.M43;
            result.M35 = matrix.M53;
            result.M36 = matrix.M63;

            result.M41 = matrix.M14;
            result.M42 = matrix.M24;
            result.M43 = matrix.M34;
            result.M44 = matrix.M44;
            result.M45 = matrix.M54;
            result.M46 = matrix.M64;

            return result;
        }
        public static Matrix4x6 Multiply(Matrix4x4 value1, Matrix4x6 value2)
        {
            Matrix4x6 result;
            // First row
            result.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21 + value1.M13 * value2.M31 + value1.M14 * value2.M41;
            result.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22 + value1.M13 * value2.M32 + value1.M14 * value2.M42;
            result.M13 = value1.M11 * value2.M13 + value1.M12 * value2.M23 + value1.M13 * value2.M33 + value1.M14 * value2.M43;
            result.M14 = value1.M11 * value2.M14 + value1.M12 * value2.M24 + value1.M13 * value2.M34 + value1.M14 * value2.M44;
            result.M15 = value1.M11 * value2.M15 + value1.M12 * value2.M25 + value1.M13 * value2.M35 + value1.M14 * value2.M45;
            result.M16 = value1.M11 * value2.M16 + value1.M12 * value2.M26 + value1.M13 * value2.M36 + value1.M14 * value2.M46;
            
            // Second row
            result.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21 + value1.M23 * value2.M31 + value1.M24 * value2.M41;
            result.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22 + value1.M23 * value2.M32 + value1.M24 * value2.M42;
            result.M23 = value1.M21 * value2.M13 + value1.M22 * value2.M23 + value1.M23 * value2.M33 + value1.M24 * value2.M43;
            result.M24 = value1.M21 * value2.M14 + value1.M22 * value2.M24 + value1.M23 * value2.M34 + value1.M24 * value2.M44;
            result.M25 = value1.M21 * value2.M15 + value1.M22 * value2.M25 + value1.M23 * value2.M35 + value1.M24 * value2.M45;
            result.M26 = value1.M21 * value2.M16 + value1.M22 * value2.M26 + value1.M23 * value2.M36 + value1.M24 * value2.M46;

            // Third row
            result.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value1.M33 * value2.M31 + value1.M34 * value2.M41;
            result.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value1.M33 * value2.M32 + value1.M34 * value2.M42;
            result.M33 = value1.M31 * value2.M13 + value1.M32 * value2.M23 + value1.M33 * value2.M33 + value1.M34 * value2.M43;
            result.M34 = value1.M31 * value2.M14 + value1.M32 * value2.M24 + value1.M33 * value2.M34 + value1.M34 * value2.M44;
            result.M35 = value1.M31 * value2.M15 + value1.M32 * value2.M25 + value1.M33 * value2.M35 + value1.M34 * value2.M45;
            result.M36 = value1.M31 * value2.M16 + value1.M32 * value2.M26 + value1.M33 * value2.M36 + value1.M34 * value2.M46;

            // Fourth row
            result.M41 = value1.M41 * value2.M11 + value1.M42 * value2.M21 + value1.M43 * value2.M31 + value1.M44 * value2.M41;
            result.M42 = value1.M41 * value2.M12 + value1.M42 * value2.M22 + value1.M43 * value2.M32 + value1.M44 * value2.M42;
            result.M43 = value1.M41 * value2.M13 + value1.M42 * value2.M23 + value1.M43 * value2.M33 + value1.M44 * value2.M43;
            result.M44 = value1.M41 * value2.M14 + value1.M42 * value2.M24 + value1.M43 * value2.M34 + value1.M44 * value2.M44;
            result.M45 = value1.M41 * value2.M15 + value1.M42 * value2.M25 + value1.M43 * value2.M35 + value1.M44 * value2.M45;
            result.M46 = value1.M41 * value2.M16 + value1.M42 * value2.M26 + value1.M43 * value2.M36 + value1.M44 * value2.M46;

            return result;
        }
        public static Matrix4x6 operator *(Matrix4x4 value1, Matrix4x6 value2)
        {
            return Multiply(value1, value2);
        }
        public static Matrix4x6 Multiply(Matrix4x6 value1, Matrix6x6 value2)
        {
            Matrix4x6 result;
            // First row
            result.M11 = value1.M11 * value2.M11 + value1.M12 * value2.M21 + value1.M13 * value2.M31 + value1.M14 * value2.M41 + value1.M15 * value2.M51 + value1.M16 * value2.M61;
            result.M12 = value1.M11 * value2.M12 + value1.M12 * value2.M22 + value1.M13 * value2.M32 + value1.M14 * value2.M42 + value1.M15 * value2.M52 + value1.M16 * value2.M62;
            result.M13 = value1.M11 * value2.M13 + value1.M12 * value2.M23 + value1.M13 * value2.M33 + value1.M14 * value2.M43 + value1.M15 * value2.M53 + value1.M16 * value2.M63;
            result.M14 = value1.M11 * value2.M14 + value1.M12 * value2.M24 + value1.M13 * value2.M34 + value1.M14 * value2.M44 + value1.M15 * value2.M54 + value1.M16 * value2.M64;
            result.M15 = value1.M11 * value2.M15 + value1.M12 * value2.M25 + value1.M13 * value2.M35 + value1.M14 * value2.M45 + value1.M15 * value2.M55 + value1.M16 * value2.M65;
            result.M16 = value1.M11 * value2.M16 + value1.M12 * value2.M26 + value1.M13 * value2.M36 + value1.M14 * value2.M46 + value1.M15 * value2.M56 + value1.M16 * value2.M66;

            // Second row
            result.M21 = value1.M21 * value2.M11 + value1.M22 * value2.M21 + value1.M23 * value2.M31 + value1.M24 * value2.M41 + value1.M25 * value2.M51 + value1.M26 * value2.M61;
            result.M22 = value1.M21 * value2.M12 + value1.M22 * value2.M22 + value1.M23 * value2.M32 + value1.M24 * value2.M42 + value1.M25 * value2.M52 + value1.M26 * value2.M62;
            result.M23 = value1.M21 * value2.M13 + value1.M22 * value2.M23 + value1.M23 * value2.M33 + value1.M24 * value2.M43 + value1.M25 * value2.M53 + value1.M26 * value2.M63;
            result.M24 = value1.M21 * value2.M14 + value1.M22 * value2.M24 + value1.M23 * value2.M34 + value1.M24 * value2.M44 + value1.M25 * value2.M54 + value1.M26 * value2.M64;
            result.M25 = value1.M21 * value2.M15 + value1.M22 * value2.M25 + value1.M23 * value2.M35 + value1.M24 * value2.M45 + value1.M25 * value2.M55 + value1.M26 * value2.M65;
            result.M26 = value1.M21 * value2.M16 + value1.M22 * value2.M26 + value1.M23 * value2.M36 + value1.M24 * value2.M46 + value1.M25 * value2.M56 + value1.M26 * value2.M66;

            // Third row
            result.M31 = value1.M31 * value2.M11 + value1.M32 * value2.M21 + value1.M33 * value2.M31 + value1.M34 * value2.M41 + value1.M35 * value2.M51 + value1.M36 * value2.M61;
            result.M32 = value1.M31 * value2.M12 + value1.M32 * value2.M22 + value1.M33 * value2.M32 + value1.M34 * value2.M42 + value1.M35 * value2.M52 + value1.M36 * value2.M62;
            result.M33 = value1.M31 * value2.M13 + value1.M32 * value2.M23 + value1.M33 * value2.M33 + value1.M34 * value2.M43 + value1.M35 * value2.M53 + value1.M36 * value2.M63;
            result.M34 = value1.M31 * value2.M14 + value1.M32 * value2.M24 + value1.M33 * value2.M34 + value1.M34 * value2.M44 + value1.M35 * value2.M54 + value1.M36 * value2.M64;
            result.M35 = value1.M31 * value2.M15 + value1.M32 * value2.M25 + value1.M33 * value2.M35 + value1.M34 * value2.M45 + value1.M35 * value2.M55 + value1.M36 * value2.M65;
            result.M36 = value1.M31 * value2.M16 + value1.M32 * value2.M26 + value1.M33 * value2.M36 + value1.M34 * value2.M46 + value1.M35 * value2.M56 + value1.M36 * value2.M66;

            // Fourth row
            result.M41 = value1.M41 * value2.M11 + value1.M42 * value2.M21 + value1.M43 * value2.M31 + value1.M44 * value2.M41 + value1.M45 * value2.M51 + value1.M46 * value2.M61;
            result.M42 = value1.M41 * value2.M12 + value1.M42 * value2.M22 + value1.M43 * value2.M32 + value1.M44 * value2.M42 + value1.M45 * value2.M52 + value1.M46 * value2.M62;
            result.M43 = value1.M41 * value2.M13 + value1.M42 * value2.M23 + value1.M43 * value2.M33 + value1.M44 * value2.M43 + value1.M45 * value2.M53 + value1.M46 * value2.M63;
            result.M44 = value1.M41 * value2.M14 + value1.M42 * value2.M24 + value1.M43 * value2.M34 + value1.M44 * value2.M44 + value1.M45 * value2.M54 + value1.M46 * value2.M64;
            result.M45 = value1.M41 * value2.M15 + value1.M42 * value2.M25 + value1.M43 * value2.M35 + value1.M44 * value2.M45 + value1.M45 * value2.M55 + value1.M46 * value2.M65;
            result.M46 = value1.M41 * value2.M16 + value1.M42 * value2.M26 + value1.M43 * value2.M36 + value1.M44 * value2.M46 + value1.M45 * value2.M56 + value1.M46 * value2.M66;


            return result;
        }
        public static Matrix4x6 operator *(Matrix4x6 value1, Matrix6x6 value2)
        {
            return Multiply(value1, value2);
        }
        public static void test()
        {
            void test1()
            {
                Matrix4x6 matrix4X6 = new Matrix4x6(
                    1, 2, -4, 1, 0, 4,
                    3, 5, -1, 2, 1, -2,
                    0, -3, 2, 5, 3, -4,
                    1, 7, 6, 9, 1, 0
                    );
                Matrix4x4 matrix4X4 = new Matrix4x4(
                    -9, -18, 7, 1,
                    -4, 19, 10, 5,
                    7, 2, -2, 6,
                    6, 72, -3, -9
                    );
                Matrix4x6 expected = new Matrix4x6(
                    -62, -122, 74, -1, 4, -28,
                    58, 92, 47, 129, 54, -94,
                    19, 72, 2, 55, 2, 32,
                    213, 318, -156, 54, 54, -108
                    );

                Matrix4x6 result = Multiply(matrix4X4, matrix4X6);
                if (!result.Equals(expected))
                {
                    throw new System.Exception("matrix4x4 matrix4x6 multiply error");
                }
                else
                {
                    Trace.WriteLine("matrix4x4 matrix4x6 multiply correct");
                    Console.WriteLine("matrix4x4 matrix4x6 multiply correct");
                }
            }
            void test2()
            {
                Matrix6x6 matrix6X6 = new Matrix6x6(
                    1, 2, -4, 1, 12, -5,
                    3, 5, -1, 2, 2, 0,
                    0, -3, 2, 5, -21, 12,
                    1, 7, 6, 9, 3, 9,
                    3, 5, -1, 2, -1, -9,
                    0, -3, 2, 5, -8, 5
                    );
                Matrix4x6 matrix4X6 = new Matrix4x6(
                    1, 2, 3, 5, 1, 0,
                    -1, 4, 0, -2, 5, 3,
                    2, 7, -1, 0, -1, 4,
                    1, 2, 3, 5, 1, 0
                    );
                Matrix4x6 expected = new Matrix4x6(
                    15,  43,  29,  67, - 33, 67,
                    24, 20, - 11, 14, - 39, - 43,
                    20,  25, - 8,  29,  28,  7,
                    15,  43,  29,  67, - 33, 67
                    );
                Matrix4x6 result = Multiply(matrix4X6, matrix6X6);
                if (!result.Equals(expected))
                {
                    throw new System.Exception("matrix4x6 matrix6x6 multiply error");
                }
                else
                {
                    Trace.WriteLine("matrix4x6 matrix6x6 multiply correct");
                    Console.WriteLine("matrix4x6 matrix6x6 multiply correct");
                }
            }
            test1();
            test2();
        }
    }
}

