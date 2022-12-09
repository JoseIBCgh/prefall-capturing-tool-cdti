

namespace ibcdatacsharp.EFK
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
    }
}
