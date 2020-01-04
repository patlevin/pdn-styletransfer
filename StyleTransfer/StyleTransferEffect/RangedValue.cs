// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    using System;

    /// <summary>
    /// Value that is limited to a given range
    /// </summary>
    /// <typeparam name="T">Value type</typeparam>
    public class RangedValue<T> where T: IComparable<T>
    {
        /// <summary>
        /// Initialise from value and limits
        /// </summary>
        /// <param name="value">Initial value</param>
        /// <param name="min">Lower range limit (inclusive)</param>
        /// <param name="max">Upper range limit (inclusive)</param>
        public RangedValue(T value, T min, T max)
        {
            if (min.CompareTo(max) > 0)
            {
                (min, max) = (max, min);
            }

            Minimum = min;
            Maximum = max;
            Value = value;
        }

        /// <summary>
        /// Get or set the wrapped value
        /// </summary>
        public T Value
        {
            get => data;
            set => data = value.Clamp<T>(Minimum, Maximum);
        }

        /// <summary>
        /// Get the lower limit
        /// </summary>
        public T Minimum { get; }

        /// <summary>
        /// Get the upper limit
        /// </summary>
        public T Maximum { get; }

        private T data;
    }
}
