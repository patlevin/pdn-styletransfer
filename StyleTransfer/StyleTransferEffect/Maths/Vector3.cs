using System.Collections;
using System.Collections.Generic;

namespace PaintDotNet.Effects.ML.StyleTransfer.Maths
{
    /// <summary>
    /// 3-Element vector class.
    /// </summary>
    public partial struct Vector3 : IVector3
    {
        /// <summary>
        /// Number of elements.
        /// </summary>
        public const int N = 3;

        /// <summary>
        /// Return a zero vector.
        /// </summary>
        public static IVector3 Zero => new Vector3(new float[N]);

        /// <summary>
        /// Create an instance from data.
        /// </summary>
        /// <param name="data">Float array of size 3; Instance takes ownership.</param>
        public Vector3(float[] data)
        {
            d = data;
        }

        /// <summary>
        /// Intiailise from elements.
        /// </summary>
        /// <param name="x">First element value</param>
        /// <param name="y">Second element value</param>
        /// <param name="z">Third element value</param>
        public Vector3(float x, float y, float z)
        {
            d = new float[] { x, y, z };
        }

        /// <summary>
        /// Get or set the element at an index.
        /// </summary>
        public float this[int index]
        {
            get => d[index];
            set => d[index] = value;
        }

        /// <summary>
        /// Test another object for equality.
        /// </summary>
        /// <param name="other">Other object to test for equality</param>
        /// <param name="comparer">Comparer that tests elements for equality</param>
        /// <returns><c>true</c>, iff other is an IVector3 with matching elements</returns>
        public bool Equals(object other, IEqualityComparer comparer)
        {
            if (!(other is IVector3))
                return false;

            var that = (IVector3)other;
            return comparer.Equals(this[0], that[0]) &&
                   comparer.Equals(this[1], that[1]) &&
                   comparer.Equals(this[2], that[2]);
        }

        /// <summary>
        /// Return the hash code of the vector based on a given criterion.
        /// </summary>
        /// <param name="comparer">Comparer that returns the hash code of each element</param>
        /// <returns>Hash code of the vector</returns>
        public int GetHashCode(IEqualityComparer comparer)
        {
            return Hashes.Default(
                comparer.GetHashCode(this[0]),
                comparer.GetHashCode(this[1]),
                comparer.GetHashCode(this[2]));
        }

        /// <summary>
        /// Test a vector for equality
        /// </summary>
        /// <param name="other">Vector to test for equality</param>
        /// <returns><c>true</c>, iff other's elements are equal to this vector's elements</returns>
        public bool Equals(IVector3 other)
        {
            return Equals(other, EqualityComparer<float>.Default);
        }

        /// <summary>
        /// Return whether another object is a vector with equal elements
        /// </summary>
        /// <param name="obj">Object to test for equality</param>
        /// <returns><c>true</c>, iff other is a vector with equal elements</returns>
        public override bool Equals(object obj)
        {
            return Equals((IVector3)obj);
        }

        /// <summary>
        /// Return the hash code of the vector
        /// </summary>
        /// <returns>Hash code of the vector elements</returns>
        public override int GetHashCode()
        {
            return Hashes.Default(
                d[0].GetHashCode(),
                d[1].GetHashCode(),
                d[2].GetHashCode());
        }

        public static bool operator ==(Vector3 a, IVector3 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector3 a, IVector3 b)
        {
            return !a.Equals(b);
        }

        private readonly float[] d;
    }
}
