namespace PaintDotNet.Effects.ML.StyleTransfer.Test
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Reflection;

    using Microsoft.ML.OnnxRuntime.Tensors;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SurfaceExtensionsTest
    {
        internal static Bitmap LoadResource(string name)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(SurfaceExtensionsTest), "resources." + name))
            {
                return new Bitmap(stream);
            }
        }

        internal static Surface LoadSurface(Bitmap bitmap)
        {
            var surface = new Surface(bitmap.Size);
            var all = new Rectangle(Point.Empty, bitmap.Size);
            var data = bitmap.LockBits(all, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var buffer = new byte[Math.Min(surface.Stride, data.Stride)];
            for (int i = 0; i < data.Height; ++i)
            {
                _ = data.GetRowArgb(buffer, 0, i);
                surface.SetRowArgb(buffer, 0, i);
            }
            bitmap.UnlockBits(data);

            return surface;
        }

        [TestMethod]
        public void TestCopyToTensor()
        {
            using (var bitmap = LoadResource("style.png"))
            using (var surface = LoadSurface(bitmap))
            {
                var portion = new Rectangle(8, 4, 16, 32);
                var tensor = new DenseTensor<float>(portion.ToNHWC());
                _ = surface.CopyToTensor(tensor, portion);

                var data = bitmap.LockBits(portion, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                var expected = new byte[data.Stride];
                var actual = new byte[expected.Length];

                for (int i = 0; i < data.Height; ++i)
                {
                    _ = data.GetRowArgb(expected, 0, i);
                    _ = tensor.GetRowArgb(actual, 0, i);
                    CollectionAssert.AreEqual(expected, actual);
                }

                bitmap.UnlockBits(data);
            }
        }

        [TestMethod]
        public void TestGetRowArgbArray()
        {
            using (var bitmap = LoadResource("style.png"))
            using (var surface = LoadSurface(bitmap))
            {
                var segment = new Rectangle(17, 4, 24, 1);
                var expected = new byte[segment.Width * 4];
                var data = bitmap.LockBits(segment, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                data.GetRowArgb(expected, 0, 0);
                bitmap.UnlockBits(data);

                var actual = new byte[expected.Length];
                _ = surface.GetRowArgb(actual, segment.X, segment.Y);

                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestGetRowArgbArraySegment()
        {
            using (var bitmap = LoadResource("style.png"))
            using (var surface = LoadSurface(bitmap))
            {
                var segment = new Rectangle(17, 4, 24, 1);
                var expected = new byte[segment.Width * 4];
                var data = bitmap.LockBits(segment, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                data.GetRowArgb(expected, 0, 0);
                bitmap.UnlockBits(data);

                var memory = new byte[expected.Length + 32];
                var actual = new ArraySegment<byte>(memory, 16, expected.Length);
                _ = surface.GetRowArgb(actual, segment.X, segment.Y);

                CollectionAssert.AreEqual(expected, actual.AsMemory().ToArray());
            }
        }

        [TestMethod]
        public void TestRowArgbArray()
        {
            using (var surface = new Surface(32, 16))
            {
                var expected = new byte[] { 120, 144, 23, 255, 78, 65, 113, 255 };
                surface.SetRowArgb(expected, 17, 9);

                var actual = new byte[expected.Length];
                _ = surface.GetRowArgb(actual, 17, 9);

                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestRowArgbArraySegment()
        {
            using (var surface = new Surface(32, 16))
            {
                var memory = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 120, 144, 23, 255, 78, 65, 113, 255, 0, 0 };
                var expected = new ArraySegment<byte>(memory, 8, 8);
                surface.SetRowArgb(expected, 17, 9);

                var actual = new byte[expected.Count];
                _ = surface.GetRowArgb(actual, 17, 9);

                CollectionAssert.AreEqual(expected.AsMemory().ToArray(), actual);
            }
        }
    }
}
