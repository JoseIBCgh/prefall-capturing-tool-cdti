

using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;

namespace ibcdatacsharp.EFK
{
    public struct Matrix6x6
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

        /// <summary>
        /// Value at row 5, column 1 of the matrix.
        /// </summary>
        public float M51;
        /// <summary>
        /// Value at row 5, column 2 of the matrix.
        /// </summary>
        public float M52;
        /// <summary>
        /// Value at row 5, column 3 of the matrix.
        /// </summary>
        public float M53;
        /// <summary>
        /// Value at row 5, column 4 of the matrix.
        /// </summary>
        public float M54;
        /// <summary>
        /// Value at row 5, column 5 of the matrix.
        /// </summary>
        public float M55;
        /// <summary>
        /// Value at row 5, column 6 of the matrix.
        /// </summary>
        public float M56;

        /// <summary>
        /// Value at row 6, column 1 of the matrix.
        /// </summary>
        public float M61;
        /// <summary>
        /// Value at row 6, column 2 of the matrix.
        /// </summary>
        public float M62;
        /// <summary>
        /// Value at row 6, column 3 of the matrix.
        /// </summary>
        public float M63;
        /// <summary>
        /// Value at row 6, column 4 of the matrix.
        /// </summary>
        public float M64;
        /// <summary>
        /// Value at row 6, column 5 of the matrix.
        /// </summary>
        public float M65;
        /// <summary>
        /// Value at row 6, column 6 of the matrix.
        /// </summary>
        public float M66;
        #endregion Public Fields
        public Matrix6x6(float m11, float m12, float m13, float m14, float m15, float m16,
                         float m21, float m22, float m23, float m24, float m25, float m26,
                         float m31, float m32, float m33, float m34, float m35, float m36,
                         float m41, float m42, float m43, float m44, float m45, float m46,
                         float m51, float m52, float m53, float m54, float m55, float m56,
                         float m61, float m62, float m63, float m64, float m65, float m66)
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

            this.M51 = m51;
            this.M52 = m52;
            this.M53 = m53;
            this.M54 = m54;
            this.M55 = m55;
            this.M56 = m56;

            this.M61 = m61;
            this.M62 = m62;
            this.M63 = m63;
            this.M64 = m64;
            this.M65 = m65;
            this.M66 = m66;
        }
        public Matrix6x6(Matrix3x3 m11, Matrix3x3 m12, Matrix3x3 m21, Matrix3x3 m22)
        {
            this.M11 = m11.M11;
            this.M12 = m11.M12;
            this.M13 = m11.M13;
            this.M14 = m12.M11;
            this.M15 = m12.M12;
            this.M16 = m12.M13;

            this.M21 = m11.M21;
            this.M22 = m11.M22;
            this.M23 = m11.M23;
            this.M24 = m12.M21;
            this.M25 = m12.M22;
            this.M26 = m12.M23;

            this.M31 = m11.M31;
            this.M32 = m11.M32;
            this.M33 = m11.M33;
            this.M34 = m12.M31;
            this.M35 = m12.M32;
            this.M36 = m12.M33;

            this.M41 = m21.M11;
            this.M42 = m21.M12;
            this.M43 = m21.M13;
            this.M44 = m22.M11;
            this.M45 = m22.M12;
            this.M46 = m22.M13;

            this.M51 = m21.M21;
            this.M52 = m21.M22;
            this.M53 = m21.M23;
            this.M54 = m22.M21;
            this.M55 = m22.M22;
            this.M56 = m22.M23;

            this.M61 = m21.M31;
            this.M62 = m21.M32;
            this.M63 = m21.M33;
            this.M64 = m22.M31;
            this.M65 = m22.M32;
            this.M66 = m22.M33;
        }
        public bool Equals(Matrix6x6 other)
        {
            return (M11 == other.M11 && M22 == other.M22 && M33 == other.M33 && M44 == other.M44 && M55 == other.M55 && M66 == other.M66 && // Check diagonal element first for early out.
                                        M12 == other.M12 && M13 == other.M13 && M14 == other.M14 && M15 == other.M15 && M16 == other.M16 &&
                    M21 == other.M21 && M23 == other.M23 && M24 == other.M24 && M25 == other.M25 && M26 == other.M26 &&
                    M31 == other.M31 && M32 == other.M32 && M34 == other.M34 && M35 == other.M35 && M36 == other.M36 &&
                    M41 == other.M41 && M42 == other.M42 && M43 == other.M43 && M45 == other.M45 && M46 == other.M46 &&
                    M51 == other.M51 && M52 == other.M52 && M53 == other.M53 && M54 == other.M54 && M56 == other.M56 &&
                    M61 == other.M61 && M62 == other.M62 && M63 == other.M63 && M64 == other.M64 && M65 == other.M65);
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
            Matrix6x6 matrix1 = new Matrix6x6(
                    -8, -16, 10, 6, 16, -10,
                    -2, 23, 16, 15, 31, 11,
                    12, 12, 13, 31, -12, -1,
                    15, 90, 24, 36, 39, 45,
                    -2, 23, 16, 15, 31, 11,
                    12, 12, 13, 31, -12, -1
                    );
            Matrix6x6 matrix2 = new Matrix6x6(
                    -8, -16, 10, 6, 16, -10,
                    -2, 23, 16, 15, 31, 11,
                    12, 12, 13, 31, -12, -1,
                    15, 90, 24, 36, 39, 45,
                    -2, 23, 16, 15, 31, 11,
                    12, 12, 13, 31, -12, -1
                    );
            Matrix6x6 matrix3 = new Matrix6x6(
                    -8, -16, 10, 6, 16, -10,
                    -2, 23, 16, 15, 31, 11,
                    12, 12, 13, 31, -12, -1,
                    15, 90, 24, 36, 39, 45,
                    -2, 23, 16, 15, 31, 11,
                    12, 12, 13, 31, -12, -2
                    );
            if (matrix1.Equals(matrix2) && matrix2.Equals(matrix1) &&
                !matrix1.Equals(matrix3) && !matrix3.Equals(matrix2))
            {
                Trace.WriteLine("matrix6x6 equals correct");
            }
            else
            {
                throw new System.Exception("matrix6x6 equals error");
            }
        }
    }
}
