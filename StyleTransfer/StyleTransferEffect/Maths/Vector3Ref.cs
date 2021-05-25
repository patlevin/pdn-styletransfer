using System.Collections;
using System.Collections.Generic;

namespace PaintDotNet.Effects.ML.StyleTransfer.Maths
{
    /// <summary>
    /// A vector that references external memory (e.g. a matrix row or -column)
    /// </summary>
    public class Vector3Ref : IVector3
    {
        /// <summary>
        /// Intiailize a vector from memory, location, and access information.
        /// </summary>
        /// <param name="data">Memory location containg the elements</param>
        /// <param name="offset">Index of the first vector element</param>
        /// <param name="stride">Distance between vector elements</param>
        public Vector3Ref(float[] data, int offset, int stride)
        {
            this.data = data;
            this.offset = offset;
            this.stride = stride;
        }

        /// <summary>
        /// Get or set the element at an index.
        /// </summary>
        public float this[int index]
        {
            get => data[offset + index * stride];
            set => data[offset + index * stride] = value;
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
            return Hashes.Simple(
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
            return Hashes.Simple(
                this[0].GetHashCode(),
                this[1].GetHashCode(),
                this[2].GetHashCode());
        }

        private readonly float[] data;
        private readonly int offset;
        private readonly int stride;
    }
}
