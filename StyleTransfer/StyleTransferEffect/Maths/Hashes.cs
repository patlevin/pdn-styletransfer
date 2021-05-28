using System;

namespace PaintDotNet.Effects.ML.StyleTransfer.Maths
{
    /// <summary>
    /// Some common hash functions for combining hashes
    /// </summary>
    public static class Hashes
    {
        /// <summary>
        /// FNV1 octet-wise hashing, see https://en.wikipedia.org/wiki/Fowler–Noll–Vo_hash_function
        /// </summary>
        /// <param name="a">First hash</param>
        /// <param name="b">Second hash</param>
        /// <param name="c">Third hash</param>
        /// <returns>Combined FNV-1a hash of (a, b,c)</returns>
        public static int FNV1(int a, int b, int c)
        {
            const int Prime = 16777619;
            const int Basis = unchecked((int)2166136261);

            int hash = Basis;

            hash = (hash * Prime) ^ ((a >> 24) & 255);
            hash = (hash * Prime) ^ ((a >> 16) & 255);
            hash = (hash * Prime) ^ ((a >> 8) & 255);
            hash = (hash * Prime) ^ ((a >> 0) & 255);

            hash = (hash * Prime) ^ ((b >> 24) & 255);
            hash = (hash * Prime) ^ ((b >> 16) & 255);
            hash = (hash * Prime) ^ ((b >> 8) & 255);
            hash = (hash * Prime) ^ ((b >> 0) & 255);

            hash = (hash * Prime) ^ ((c >> 24) & 255);
            hash = (hash * Prime) ^ ((c >> 16) & 255);
            hash = (hash * Prime) ^ ((c >> 8) & 255);
            hash = (hash * Prime) ^ ((c >> 0) & 255);

            return hash;
        }

        /// <summary>
        /// Return combined hash of (a, b, c) using a simple hash function
        /// </summary>
        /// <param name="a">First hash</param>
        /// <param name="b">Second hash</param>
        /// <param name="c">Third hash</param>
        /// <returns>Combined hash of (a, b, c) using a simple hash function</returns>
        public static int Simple(int a, int b, int c)
        {
            const int Prime = 37;
            const int Basis = 17;

            var hash = Basis;
            hash = hash * Prime + a;
            hash = hash * Prime + b;
            hash = hash * Prime + c;

            return hash;
        }

        /// <summary>
        /// Default hash function.
        /// </summary>
        public static Func<int, int, int, int> Default = Simple;
    }
}
