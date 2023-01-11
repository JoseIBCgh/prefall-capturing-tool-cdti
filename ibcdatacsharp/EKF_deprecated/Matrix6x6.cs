

using ibcdatacsharp.UI.Common;
using MS.WindowsAPICodePack.Internal;
using ScottPlot.Styles;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace ibcdatacsharp.EFK_old
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
        public static Matrix6x6 Identity()
        {
            Matrix6x6 result = new Matrix6x6(
                1, 0, 0, 0, 0, 0,
                0, 1, 0, 0, 0, 0,
                0, 0, 1, 0, 0, 0,
                0, 0, 0, 1, 0, 0,
                0, 0, 0, 0, 1, 0,
                0, 0, 0, 0, 0, 1
                );
            return result;
        }
        public static float[,] ToArray(Matrix6x6 m)
        {
            float[,] result = new float[6, 6];
            result[0, 0] = m.M11;
            result[0, 1] = m.M12;
            result[0, 2] = m.M13;
            result[0, 3] = m.M14;
            result[0, 4] = m.M15;
            result[0, 5] = m.M16;

            result[1, 0] = m.M21;
            result[1, 1] = m.M22;
            result[1, 2] = m.M23;
            result[1, 3] = m.M24;
            result[1, 4] = m.M25;
            result[1, 5] = m.M26;

            result[2, 0] = m.M31;
            result[2, 1] = m.M32;
            result[2, 2] = m.M33;
            result[2, 3] = m.M34;
            result[2, 4] = m.M35;
            result[2, 5] = m.M36;

            result[3, 0] = m.M41;
            result[3, 1] = m.M42;
            result[3, 2] = m.M43;
            result[3, 3] = m.M44;
            result[3, 4] = m.M45;
            result[3, 5] = m.M46;

            result[4, 0] = m.M51;
            result[4, 1] = m.M52;
            result[4, 2] = m.M53;
            result[4, 3] = m.M54;
            result[4, 4] = m.M55;
            result[4, 5] = m.M56;

            result[5, 0] = m.M61;
            result[5, 1] = m.M62;
            result[5, 2] = m.M63;
            result[5, 3] = m.M64;
            result[5, 4] = m.M65;
            result[5, 5] = m.M66;

            return result;
        }
        public static Matrix6x6 FromArray(float[,] m)
        {
            Matrix6x6 result;
            result.M11 = m[0, 0];
            result.M12 = m[0, 1];
            result.M13 = m[0, 2];
            result.M14 = m[0, 3];
            result.M15 = m[0, 4];
            result.M16 = m[0, 5];

            result.M21 = m[1, 0];
            result.M22 = m[1, 1];
            result.M23 = m[1, 2];
            result.M24 = m[1, 3];
            result.M25 = m[1, 4];
            result.M26 = m[1, 5];

            result.M31 = m[2, 0];
            result.M32 = m[2, 1];
            result.M33 = m[2, 2];
            result.M34 = m[2, 3];
            result.M35 = m[2, 4];
            result.M36 = m[2, 5];

            result.M41 = m[3, 0];
            result.M42 = m[3, 1];
            result.M43 = m[3, 2];
            result.M44 = m[3, 3];
            result.M45 = m[3, 4];
            result.M46 = m[3, 5];

            result.M51 = m[4, 0];
            result.M52 = m[4, 1];
            result.M53 = m[4, 2];
            result.M54 = m[4, 3];
            result.M55 = m[4, 4];
            result.M56 = m[4, 5];

            result.M61 = m[5, 0];
            result.M62 = m[5, 1];
            result.M63 = m[5, 2];
            result.M64 = m[5, 3];
            result.M65 = m[5, 4];
            result.M66 = m[5, 5];

            return result;
        }
        public static Matrix6x6 Add(Matrix6x6 value1, Matrix6x6 value2)
        {
            Matrix6x6 result;

            result.M11 = value1.M11 + value2.M11;
            result.M12 = value1.M12 + value2.M12;
            result.M13 = value1.M13 + value2.M13;
            result.M14 = value1.M14 + value2.M14;
            result.M15 = value1.M15 + value2.M15;
            result.M16 = value1.M16 + value2.M16;

            result.M21 = value1.M21 + value2.M21;
            result.M22 = value1.M22 + value2.M22;
            result.M23 = value1.M23 + value2.M23;
            result.M24 = value1.M24 + value2.M24;
            result.M25 = value1.M25 + value2.M25;
            result.M26 = value1.M26 + value2.M26;

            result.M31 = value1.M31 + value2.M31;
            result.M32 = value1.M32 + value2.M32;
            result.M33 = value1.M33 + value2.M33;
            result.M34 = value1.M34 + value2.M34;
            result.M35 = value1.M35 + value2.M35;
            result.M36 = value1.M36 + value2.M36;

            result.M41 = value1.M41 + value2.M41;
            result.M42 = value1.M42 + value2.M42;
            result.M43 = value1.M43 + value2.M43;
            result.M44 = value1.M44 + value2.M44;
            result.M45 = value1.M45 + value2.M45;
            result.M46 = value1.M46 + value2.M46;

            result.M51 = value1.M51 + value2.M51;
            result.M52 = value1.M52 + value2.M52;
            result.M53 = value1.M53 + value2.M53;
            result.M54 = value1.M54 + value2.M54;
            result.M55 = value1.M55 + value2.M55;
            result.M56 = value1.M56 + value2.M56;

            result.M61 = value1.M61 + value2.M61;
            result.M62 = value1.M62 + value2.M62;
            result.M63 = value1.M63 + value2.M63;
            result.M64 = value1.M64 + value2.M64;
            result.M65 = value1.M65 + value2.M65;
            result.M66 = value1.M66 + value2.M66;

            return result;
        }
        public static Matrix6x6 operator +(Matrix6x6 value1, Matrix6x6 value2)
        {
            return Add(value1, value2);
        }
        public static bool Invert(Matrix6x6 matrix, out Matrix6x6 result)
        {
            float[,] matrixTemp = ToArray(matrix);
            float[,] resultTemp = Gaus.Inverse(ref matrixTemp);
            result = FromArray(resultTemp);
            //Trace.WriteLine(FromArray(matrixTemp));
            //return Error(FromArray(matrixTemp), Identity()) < 0.01f;
            return FromArray(matrixTemp).NearlyEqual(Identity());
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
            if (obj is Matrix6x6)
            {
                return Equals((Matrix6x6)obj);
            }

            return false;
        }
        public bool NearlyEqual(Matrix6x6 other)
        {
            return Helpers.NearlyEqual(M11, other.M11) &&
                Helpers.NearlyEqual(M12, other.M12) &&
                Helpers.NearlyEqual(M13, other.M13) &&
                Helpers.NearlyEqual(M14, other.M14) &&
                Helpers.NearlyEqual(M15, other.M15) &&
                Helpers.NearlyEqual(M16, other.M16) &&
                Helpers.NearlyEqual(M21, other.M21) &&
                Helpers.NearlyEqual(M22, other.M22) &&
                Helpers.NearlyEqual(M23, other.M23) &&
                Helpers.NearlyEqual(M24, other.M24) &&
                Helpers.NearlyEqual(M25, other.M25) &&
                Helpers.NearlyEqual(M26, other.M26) &&
                Helpers.NearlyEqual(M31, other.M31) &&
                Helpers.NearlyEqual(M32, other.M32) &&
                Helpers.NearlyEqual(M33, other.M33) &&
                Helpers.NearlyEqual(M34, other.M34) &&
                Helpers.NearlyEqual(M35, other.M35) &&
                Helpers.NearlyEqual(M36, other.M36) &&
                Helpers.NearlyEqual(M41, other.M41) &&
                Helpers.NearlyEqual(M42, other.M42) &&
                Helpers.NearlyEqual(M43, other.M43) &&
                Helpers.NearlyEqual(M44, other.M44) &&
                Helpers.NearlyEqual(M45, other.M45) &&
                Helpers.NearlyEqual(M46, other.M46) &&
                Helpers.NearlyEqual(M51, other.M51) &&
                Helpers.NearlyEqual(M52, other.M52) &&
                Helpers.NearlyEqual(M53, other.M53) &&
                Helpers.NearlyEqual(M54, other.M54) &&
                Helpers.NearlyEqual(M55, other.M55) &&
                Helpers.NearlyEqual(M56, other.M56) &&
                Helpers.NearlyEqual(M61, other.M61) &&
                Helpers.NearlyEqual(M62, other.M62) &&
                Helpers.NearlyEqual(M63, other.M63) &&
                Helpers.NearlyEqual(M64, other.M64) &&
                Helpers.NearlyEqual(M65, other.M65) &&
                Helpers.NearlyEqual(M66, other.M66);
        }
        public override string ToString()
        {
            return M11.ToString() + ", " + M12.ToString() + ", " + M13.ToString() + ", " +
                M14.ToString() + ", " + M15.ToString() + ", " + M16.ToString() + "\n" +
                M21.ToString() + ", " + M22.ToString() + ", " + M23.ToString() + ", " +
                M24.ToString() + ", " + M25.ToString() + ", " + M26.ToString() + "\n" +
                M31.ToString() + ", " + M32.ToString() + ", " + M33.ToString() + ", " +
                M34.ToString() + ", " + M35.ToString() + ", " + M36.ToString() + "\n" +
                M41.ToString() + ", " + M42.ToString() + ", " + M43.ToString() + ", " +
                M44.ToString() + ", " + M45.ToString() + ", " + M46.ToString() + "\n" +
                M51.ToString() + ", " + M52.ToString() + ", " + M53.ToString() + ", " +
                M54.ToString() + ", " + M55.ToString() + ", " + M56.ToString() + "\n" +
                M61.ToString() + ", " + M62.ToString() + ", " + M63.ToString() + ", " +
                M64.ToString() + ", " + M65.ToString() + ", " + M66.ToString();
        }
        public static float Error(Matrix6x6 value1, Matrix6x6 value2)
        {
            float error = 0f;
            error += (value1.M11 - value2.M11);
            error += (value1.M12 - value2.M12);
            error += (value1.M13 - value2.M13);
            error += (value1.M14 - value2.M14);
            error += (value1.M15 - value2.M15);
            error += (value1.M16 - value2.M16);

            error += (value1.M21 - value2.M21);
            error += (value1.M22 - value2.M22);
            error += (value1.M23 - value2.M23);
            error += (value1.M24 - value2.M24);
            error += (value1.M25 - value2.M25);
            error += (value1.M26 - value2.M26);

            error += (value1.M31 - value2.M31);
            error += (value1.M32 - value2.M32);
            error += (value1.M33 - value2.M33);
            error += (value1.M34 - value2.M34);
            error += (value1.M35 - value2.M35);
            error += (value1.M36 - value2.M36);

            error += (value1.M41 - value2.M41);
            error += (value1.M42 - value2.M42);
            error += (value1.M43 - value2.M43);
            error += (value1.M44 - value2.M44);
            error += (value1.M45 - value2.M45);
            error += (value1.M46 - value2.M46);

            error += (value1.M51 - value2.M51);
            error += (value1.M52 - value2.M52);
            error += (value1.M53 - value2.M53);
            error += (value1.M54 - value2.M54);
            error += (value1.M55 - value2.M55);
            error += (value1.M56 - value2.M56);

            error += (value1.M61 - value2.M61);
            error += (value1.M62 - value2.M62);
            error += (value1.M63 - value2.M63);
            error += (value1.M64 - value2.M64);
            error += (value1.M65 - value2.M65);
            error += (value1.M66 - value2.M66);
            
            return error / 36f;
        }
        public static void test()
        {
            void test1()
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
            void test2()
            {
                Matrix6x6 matrix1 = new Matrix6x6(
                    -8, -16, 10, 6, 16, -10,
                    -2, 23, 16, 15, 31, 11,
                    12, 12, 13, 31, -12, -1,
                    15, 90, 24, 36, 39, 45,
                    -2, 23, 16, 15, 31, 11,
                    12, 12, 13, 31, -12, -1
                    );
                Matrix6x6 result1;
                bool exist1 = Matrix6x6.Invert(matrix1, out result1);
                if (exist1)
                {
                    throw new System.Exception("matrix6x6 inverse shouldn't exist");
                }
                else
                {
                    Trace.WriteLine("matrix6x6 inverse doesn't exist correct");
                }
                Matrix6x6 matrix2 = new Matrix6x6(
                    2, 6, 8, 6, 5, 8,
                    8, 1, 6, 2, 3, 4,
                    3, 3, 4, 6, 7, 7,
                    9, 4, 0, 9, 7, 9,
                    2, 6, 1, 9, 4, 6,
                    8, 9, 7, 6, 9, 9
                    );
                Matrix6x6 expected2 = new Matrix6x6(
- 0.070590320865094841377f,    0.10413047332033327429f, - 0.07537670625775571702f, 0.02605920936004254559f,  0.02776103527743307936f,  0.030526502393192696313f,
0.00776458074809430963f, - 0.07775217160078000376f, - 0.17252260237546534307f, - 0.01843644743839744706f, 0.04566566211664598439f,  0.14983159014359156183f,
- 0.00911185959936181522f, 0.14603793653607516386f,  0.079170359865272114852f, - 0.14731430597411806408f, 0.08796312710512320489f, - 0.029711044141109732269f,
- 0.1571707144123382377f,  0.17659989363588016278f,  0.13603970927140577896f, - 0.15192341783371742568f, 0.29937954263428470064f, - 0.092253146605211841759f,
- 0.25857117532352419775f, 0.00021272823967381649f,  0.32058145718844176555f, - 0.15316433256514802321f, 0.04091473143059741141f,  0.10629321042368374409f,
0.42542102464102109526f, - 0.24633930154227973706f, - 0.23332742421556461596f, 0.36429711044141109678f, - 0.37925899663180287068f, - 0.087537670625775571931f
                    );
                Matrix6x6 result2;
                bool exist2 = Matrix6x6.Invert(matrix2, out result2);
                if (!exist2)
                {
                    throw new Exception("matrix6x6 inverse should exist");
                }
                else
                {
                    if (result2.Equals(expected2)){
                        Trace.WriteLine("matrix inverse perfect");
                    }
                    else
                    {
                        if (expected2.NearlyEqual(result2))
                        {
                            Trace.WriteLine("matrix inverse nearly equal");
                            Trace.WriteLine("result");
                            Trace.WriteLine(result2);
                            Trace.WriteLine("expected");
                            Trace.WriteLine(expected2);
                        }
                        else
                        {
                            Trace.WriteLine("result");
                            Trace.WriteLine(result2);
                            Trace.WriteLine("expected");
                            Trace.WriteLine(expected2);
                            float error = Error(expected2, result2);
                            throw new Exception("matrix inverse error " + error + " inspect manually better");
                        }
                    }
                }
            }
            test1();
            test2();
        }
    }
}
