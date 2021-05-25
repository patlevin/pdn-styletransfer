using System;
using System.Collections;

namespace PaintDotNet.Effects.ML.StyleTransfer.Maths
{
    /// <summary>
    /// Interface for 1D-indexable objects. Implementations will have three elements.
    /// </summary>
    public interface IVector3 : IStructuralEquatable, IEquatable<IVector3>
    {
        /// <summary>
        /// Get or set a value.
        /// </summary>
        /// <param name="index">Index of the value</param>
        /// <returns>Value at the given index</returns>
        float this[int index]
        {
            get;
            set;
        }
    }
}
