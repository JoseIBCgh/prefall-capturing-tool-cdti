using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ibcdatacsharp.EFK_old
{
    public struct Vector6
    {
        public Single V1;
        public Single V2;
        public Single V3;
        public Single V4;
        public Single V5;
        public Single V6;

        #region Constructors

        /// <summary>
        /// Constructs a Vector3 from the given Vector2 and a third value.
        /// </summary>
        /// <param name="value">The Vector to extract X and Y components from.</param>
        /// <param name="z">The Z component.</param>
        public Vector6(Vector3 vector1, Vector3 vector2) : this(vector1.X, vector1.Y, vector1.Z,
            vector2.X, vector2.Y, vector2.Z) { }

        public Vector6(Single v1, Single v2, Single v3, Single v4, Single v5, Single v6)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
            V4 = v4;
            V5 = v5;
            V6 = v6;
        }
        #endregion Constructors

        #region Public Static Operators
        /// <summary>
        /// Adds two vectors together.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The summed vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector6 operator +(Vector6 left, Vector6 right)
        {
            return new Vector6(left.V1 + right.V1, left.V2 + right.V2, left.V3 + right.V3,
                left.V4 + right.V4, left.V5 + right.V5, left.V6 + right.V6);
        }

        /// <summary>
        /// Subtracts the second vector from the first.
        /// </summary>
        /// <param name="left">The first source vector.</param>
        /// <param name="right">The second source vector.</param>
        /// <returns>The difference vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector6 operator -(Vector6 left, Vector6 right)
        {
            return new Vector6(left.V1 - right.V1, left.V2 - right.V2, left.V3 - right.V3,
                left.V4 - right.V4, left.V5 - right.V5, left.V6 - right.V6);
        }

        /// <summary>
        /// Multiplies a vector by the given scalar.
        /// </summary>
        /// <param name="left">The source vector.</param>
        /// <param name="right">The scalar value.</param>
        /// <returns>The scaled vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector6 operator *(Vector6 left, Single right)
        {
            return new Vector6(left.V1 * right, left.V2 * right, left.V3 * right,
                left.V4 * right, left.V5 * right, left.V6 * right);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector6 operator *(Single left, Vector6 right)
        {
            return new Vector6(left * right.V1, left * right.V2, left * right.V3,
                left * right.V4, left * right.V5, left * right.V6);
        }
        #endregion Public Static Operators
    }
}
