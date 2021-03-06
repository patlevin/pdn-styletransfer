﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace PaintDotNet.Effects.ML.StyleTransfer.Maths
{
    /// <summary>
    /// Matrix column indexes
    /// </summary>
    public enum ColumnIndex : int
    {
        _0,
        _1,
        _2
    }

    /// <summary>
    /// Rudimentary quadratic 3x3 matrix.
    /// </summary>
    public class Matrix3 : IStructuralEquatable, IEquatable<Matrix3>
    {
        /// <summary>
        /// Number of rows/columns
        /// </summary>
        public const int N = 3;

        /// <summary>
        /// Zero matrix (0)
        /// </summary>
        public static Matrix3 Zero => new Matrix3(new float[N * N]);

        /// <summary>
        /// Return a diagonal matrix from a scalar
        /// </summary>
        /// <param name="d">Value of each diagonal element</param>
        /// <returns>Diagonal matrix of (d..d..d)</returns>
        public static Matrix3 Diag(float d)
        {
            return Diag(d, d, d);
        }

        /// <summary>
        /// Return a diagonal matrix from a vector
        /// </summary>
        /// <param name="d1">Element at M[0, 0]</param>
        /// <param name="d2">Element at M[1, 1]</param>
        /// <param name="d3">Element at M[2, 2]</param>
        /// <returns>Diagonal matrix of (d1..d2..d3)</returns>
        public static Matrix3 Diag(float d1, float d2, float d3)
        {
            return new Matrix3(new float[] { d1, 0, 0, 0, d2, 0, 0, 0, d3 });
        }

        /// <summary>
        /// Trace of the matrix (sum of diagonal elements)
        /// </summary>
        public float Trace => m[0] + m[4] + m[8];

        /// <summary>
        /// Return a wrapper for supporting operator overloading
        /// </summary>
        public Matrix3Wrapper _ => new Matrix3Wrapper(this);

        /// <summary>
        /// Get or set the element at a given position
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="col">Column index</param>
        /// <returns>Element at the given position</returns>
        public float this[int row, int col]
        {
            get => m[row * N + col];
            set => m[row * N + col] = value;
        }

        /// <summary>
        /// Return a row vector
        /// </summary>
        /// <param name="row">Row index</param>
        /// <returns>Row vector view</returns>
        public IVector3 this[int row]
        {
            get => Row(row);
            set => Row(row).Copy(value);
        }

        /// <summary>
        /// Return a column vector
        /// </summary>
        /// <param name="col">Column index</param>
        /// <returns>Column vector view</returns>
        public IVector3 this[ColumnIndex col]
        {
            get => Column((int)col);
            set => Column((int)col).Copy(value);
        }

        /// <summary>
        /// Intialize a matrix from an array
        /// </summary>
        /// <param name="data">Array of at least N*N elements.</param>
        public Matrix3(float[] data)
        {
            m = data;
        }

        /// <summary>
        /// Return the ith row
        /// </summary>
        /// <param name="i">Row index</param>
        /// <returns>Vector containg the elements of the ith row</returns>
        public IVector3 Row(int i)
        {
            return new Vector3Ref(m, i * N, 1);
        }

        /// <summary>
        /// Return the ith column
        /// </summary>
        /// <param name="i">Column index</param>
        /// <returns>Vector containg the elements of the ith column</returns>
        public IVector3 Column(int i)
        {
            return new Vector3Ref(m, i, N);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Matrix3))
                return false;

            return Equals((Matrix3)obj);
        }

        public bool Equals(Matrix3 other)
        {
            return Equals(other, EqualityComparer<float>.Default);
        }

        public bool Equals(object other, IEqualityComparer comparer)
        {
            if (!(other is Matrix3))
                return false;

            var that = (Matrix3)other;
            return this[0].Equals(that[0], comparer) &&
                   this[1].Equals(that[1], comparer) &&
                   this[2].Equals(that[2], comparer);
        }

        public override int GetHashCode()
        {
            return Hashes.Default(
                this[0].GetHashCode(),
                this[1].GetHashCode(),
                this[2].GetHashCode());
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            return Hashes.Default(
                this[0].GetHashCode(comparer),
                this[1].GetHashCode(comparer),
                this[2].GetHashCode(comparer));
        }

        internal readonly float[] m;
    }
}
