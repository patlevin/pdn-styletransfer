namespace PaintDotNet.Effects.ML.StyleTransfer.Test
{
    using Microsoft.ML.OnnxRuntime.Tensors;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;

    [TestClass]
    public class TensorExtensionsTest
    {
        [TestMethod, Description("Tensor<T>.Mix() should follow GLSL mix()-semantics")]
        public void TestMix()
        {
            var mix = new float[] { 1.75f, 1.75f, 1.75f, 1.75f };
            var x = new DenseTensor<float>(new float[] { 2f, 1f, 3f, 4f }, new int[] { 1, 2, 2, 1 });
            var y = new DenseTensor<float>(new float[] { 1f, 4f, -2f, -5f }, new int[] { 1, 2, 2, 1 });
            var z = x.Mix(y, 0.25f).ToDenseTensor().Buffer.ToArray();

            CollectionAssert.AreEqual(mix, z);
        }

        [TestMethod]
        public void TestToTensor()
        {
            using (var bitmap = SurfaceExtensionsTest.LoadResource("style.png"))
            using (var surf = new Surface(bitmap.Width, bitmap.Height, SurfaceCreationFlags.Default))
            {
                surf.CopyFromGdipBitmap(bitmap);
                using (var copy = surf.Clone())
                {
                    // round-trip: surface->tensor->surface
                    surf.ToTensor().ToSurface(copy);

                    var (cols, rows) = (surf.Width, surf.Height);

                    // compare pixel by pixel
                    for (int y = 0; y < rows; ++y)
                    {
                        for (int x = 0; x < cols; ++x)
                        {
                            Assert.AreEqual(surf[x, y], copy[x, y]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestWidth()
        {
            var tensor = new DenseTensor<float>(new int[] { 1, 4, 2, 3 });
            Assert.AreEqual(2, tensor.Width());
        }

        [TestMethod]
        public void TestHeight()
        {
            var tensor = new DenseTensor<float>(new int[] { 1, 4, 2, 3 });
            Assert.AreEqual(4, tensor.Height());
        }

        [TestMethod]
        public void TestGetRowArgbArray()
        {
            // 3x2 argb image
            var bitmap = new byte[]
            {
                20, 40, 60, 255, 30, 50, 70, 255, 40, 60, 80, 255,
                120, 140, 160, 255, 130, 150, 170, 255, 140, 160, 180, 255,
            };

            var tensorData = bitmap
                .Where((_, i) => (i + 1) % 4 != 0)
                .Select(b => b / 255f)
                .ToArray();

            for (int i = 0; i < tensorData.Length/3; ++i)
            {
                (tensorData[i * 3 + 0], tensorData[i * 3 + 2]) = (tensorData[i * 3 + 2], tensorData[i * 3 + 0]);
            }

            var tensor = new DenseTensor<float>(tensorData, new int[] { 1, 2, 3, 3 });

            var row = new byte[12];
            var len = tensor.GetRowArgb(row, 0, 0);
            CollectionAssert.AreEqual(bitmap.Take(12).ToArray(), row.Take(len).ToArray());

            len = tensor.GetRowArgb(row, 2, 0);
            CollectionAssert.AreEqual(bitmap.Skip(8).Take(4).ToArray(), row.Take(len).ToArray());

            row = new byte[8];
            len = tensor.GetRowArgb(row, 0, 1);
            CollectionAssert.AreEqual(bitmap.Skip(12).Take(8).ToArray(), row.Take(len).ToArray());

            len = tensor.GetRowArgb(row, 2, 1);
            CollectionAssert.AreEqual(bitmap.Skip(20).ToArray(), row.Take(len).ToArray());
        }

        [TestMethod]
        public void TestGetRowArgbArraySegment()
        {
            // 3x2 argb image
            var bitmap = new byte[]
            {
                20, 40, 60, 255, 30, 50, 70, 255, 40, 60, 80, 255,
                120, 140, 160, 255, 130, 150, 170, 255, 140, 160, 180, 255,
            };

            var tensorData = bitmap
                .Where((_, i) => (i + 1) % 4 != 0)
                .Select(b => b / 255f)
                .ToArray();

            for (int i = 0; i < tensorData.Length / 3; ++i)
            {
                (tensorData[i * 3 + 0], tensorData[i * 3 + 2]) = (tensorData[i * 3 + 2], tensorData[i * 3 + 0]);
            }

            var tensor = new DenseTensor<float>(tensorData, new int[] { 1, 2, 3, 3 });

            var data = new byte[bitmap.Length];
            var row = new ArraySegment<byte>(data, 12, 8);
            var len = tensor.GetRowArgb(row, 0, 0);
            CollectionAssert.AreEqual(bitmap.Take(row.Count).ToArray(), row.Take(len).ToArray());

            len = tensor.GetRowArgb(row, 2, 0);
            CollectionAssert.AreEqual(bitmap.Skip(8).Take(4).ToArray(), row.Take(len).ToArray());

            len = tensor.GetRowArgb(row, 0, 1);
            CollectionAssert.AreEqual(bitmap.Skip(12).Take(row.Count).ToArray(), row.Take(len).ToArray());
        }

        [TestMethod]
        public void TestSetRowArgbArray()
        {
            using (var bitmap = SurfaceExtensionsTest.LoadResource("style.png"))
            {
                var line = new Rectangle(13, 27, 64, 1);
                var data = bitmap.LockBits(line, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                var expected = new byte[line.Width * 4];
                _ = data.GetRowArgb(expected, 0, 0);
                bitmap.UnlockBits(data);

                var tensor = new DenseTensor<float>(new Rectangle(Point.Empty, bitmap.Size).ToNHWC());
                tensor.SetRowArgb(expected, line.X, line.Y);

                var actual = new byte[expected.Length];
                _ = tensor.GetRowArgb(actual, line.X, line.Y);

                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void TestSetRowArgbArraySegment()
        {
            using (var bitmap = SurfaceExtensionsTest.LoadResource("style.png"))
            {
                var line = new Rectangle(13, 27, 64, 1);
                var data = bitmap.LockBits(line, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                var expected = new byte[data.Stride + 32];
                var memory = new ArraySegment<byte>(expected, 8, line.Width * 4);
                _ = data.GetRowArgb(memory, 0, 0);
                bitmap.UnlockBits(data);

                var tensor = new DenseTensor<float>(new Rectangle(Point.Empty, bitmap.Size).ToNHWC());
                tensor.SetRowArgb(memory, line.X, line.Y);

                var actual = new byte[memory.Count];
                _ = tensor.GetRowArgb(actual, line.X, line.Y);

                CollectionAssert.AreEqual(expected.Skip(memory.Offset).Take(memory.Count).ToArray(), actual);
            }
        }

        [TestMethod]
        public void TestMixArgb()
        {
            var x = new byte[] { 120, 200, 40, 255, 240, 60, 32, 255 };
            var y = new byte[] { 40, 110, 80, 255, 150, 74, 109, 255 };
            var z = new byte[x.Length];
            var mixed = new byte[] { 104, 182, 48, 255, 222, 62, 47, 255 };

            CollectionAssert.AreEqual(mixed, x.MixArgb(y, z, 0.2f));
        }

        [TestMethod]
        public void TestMixArgbArraySegment()
        {
            var x = new byte[] { 0, 0, 120, 200, 40, 255, 240, 60, 32, 255, 0, 0 };
            var y = new byte[] { 0, 0, 0, 0, 40, 110, 80, 255, 150, 74, 109, 255 };
            var z = new byte[x.Length + 4];

            var xs = new ArraySegment<byte>(x, 2, 8);
            var ys = new ArraySegment<byte>(y, 4, 8);
            var zs = new ArraySegment<byte>(z, 3, 8);

            var mixed = new byte[] { 104, 182, 48, 255, 222, 62, 47, 255 };

            xs.MixArgb(ys, zs, 0.2f);
            CollectionAssert.AreEqual(mixed, zs.AsMemory().ToArray());
        }

        [TestMethod]
        public void TestLinearBlend()
        {
            var x = new byte[] { 0, 0, 120, 200, 40, 255, 240, 60, 32, 255 };
            var y = new byte[] { 40, 110, 80, 255, 150, 74, 109, 255 };
            var z = new byte[16];
            var mixed = new byte[] { 0, 0, 0, 0, 80, 155, 60, 255, 150, 74, 109, 255, 0, 0, 0, 0 };

            var xs = new ArraySegment<byte>(x, 2, 8);
            var ys = new ArraySegment<byte>(y);
            var zs = new ArraySegment<byte>(z, 4, 8);
            CollectionAssert.AreEqual(mixed, xs.LinearBlend(ys, zs).Array);
        }
    
        [TestMethod]
        public void TestToNHWC()
        {
            var rect = new Rectangle(3, 9, 22, 14);
            var dims = rect.ToNHWC();
            CollectionAssert.AreEqual(new int[] { 1, rect.Height, rect.Width, 3 }, dims);
        }
    }
}
