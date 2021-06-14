using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaintDotNet.Effects.ML.StyleTransfer.Dml;

namespace PaintDotNet.Effects.ML.StyleTransfer.Test
{
    [TestClass]
    public class DevicesTest
    {
        [TestMethod("SelectGpuType should detect known Intel iGPUs")]
        public void TestDetectInteliGpu()
        {
            // list extracted from Intel ARK (https://ark.intel.com/content/www/us/en/ark.html#@Processors)
            string[] KnowniGpuNames = new string[]
            {
                "Intel(R) UHD Graphics P630",
                "Intel(R) UHD Graphics",
                "Intel(R) UHD Graphics 630",
                "Intel(R) UHD Graphics 620",
                "Intel(R) UHD Graphics 617",
                "Intel(R) UHD Graphics 615",
                "Intel(R) UHD Graphics 610",
                "Intel(R) UHD Graphics 605",
                "Intel(R) UHD Graphics 600",
                "Intel(R) Iris Xe Graphics",
                "Intel(R) Iris Pro Graphics P580",
                "Intel(R) Iris Pro Graphics 6200",
                "Intel(R) Iris Pro Graphics 580",
                "Intel(R) Iris Pro Graphics 5200",
                "Intel(R) Iris Plus Graphics 655",
                "Intel(R) Iris Plus Graphics 650",
                "Intel(R) Iris Plus Graphics 645",
                "Intel(R) Iris Plus Graphics 640",
                "Intel(R) Iris Plus Graphics",
                "Intel(R) Iris Graphics 6100",
                "Intel(R) Iris Graphics 550",
                "Intel(R) Iris Graphics 540",
                "Intel(R) Iris Graphics 5100",
                "Intel(R) HD Graphics P630",
                "Intel(R) HD Graphics P530",
                "Intel(R) HD Graphics",
                "Intel(R) HD Graphics 630",
                "Intel(R) HD Graphics 620",
                "Intel(R) HD Graphics 615",
                "Intel(R) HD Graphics 610",
                "Intel(R) HD Graphics 6000",
                "Intel(R) HD Graphics 5500",
                "Intel(R) HD Graphics 5300",
                "Intel(R) HD Graphics 530",
                "Intel(R) HD Graphics 520",
                "Intel(R) HD Graphics 515",
                "Intel(R) HD Graphics 510",
                "Intel(R) HD Graphics 5000",
                "Intel(R) HD Graphics 4600",
                "Intel(R) HD Graphics 4400",
                "Intel(R) HD Graphics 4200",
                "Intel(R) HD Graphics 4000",
                "Intel(R) HD Graphics 3000",
                "Intel(R) HD Graphics 2500",
                "Intel(R) HD Graphics 2000",
            };

            bool IsIntegratedGpu(string name)
            {
                return Devices.QueryDeviceType(name, VendorId.Intel) == DeviceType.Igp;
            }

            foreach (var name in KnowniGpuNames)
            {
                Assert.IsTrue(IsIntegratedGpu(name), $"expected '{name}' to match");
            }
        }

        [TestMethod("SelectGpuType should detect known Intel dGPU")]
        public void TestDetectInteldGpu()
        {
            var isdGpu = Devices.QueryDeviceType("Iris Xe Max", VendorId.Intel) ==
                DeviceType.Dgpu;
            Assert.IsTrue(isdGpu);
        }

        [TestMethod("SelectGpuType should detect known AMD APUs")]
        public void TestDetectAmdApu()
        {
            // list exported from AMD Processor Specifications (https://www.amd.com/en/products/specifications/processors)
            string[] KnownApuNames = new string[]
            {
                "AMD Radeon Graphics",
                "Radeon  Graphics",
                "Radeon Graphics",
                "Radeon RX Vega 11 Graphics",
                "Radeon Vega 11 Graphics",
                "Radeon Vega 8 Graphics",
                "Radeon Vega 3 Graphics",
                "Radeon RX Vega 10 Graphics",
                "Radeon Vega 6 Graphics",
                "Radeon Vega 9 Graphics",
                "Radeon Vega 10 Graphics",
                "AMD Radeon R5 Graphics",
                "AMD Radeon R4 Graphics",
                "Radeon R7 Series",
                "Radeon R5 Series",
                "AMD Radeon R7 Graphics",
                "Radeon R5 Graphics",
                "AMD Radeon R3 Graphics",
                "Radeon R4 Graphics",
                "R7",
                "R5",
                "AMD Radeon R6 Graphics",
                "AMD Radeon HD 8670D",
                "AMD Radeon HD 8650D",
                "AMD Radeon HD 8570D",
                "AMD Radeon HD 8550D",
                "AMD Radeon HD 8470D",
                "AMD Radeon R4 Series",
                "AMD Radeon HD 8400",
                "AMD Radeon HD 8370D",
                "AMD Radeon HD 8330",
                "AMD Radeon HD 8450G",
                "AMD Radeon HD 8240 Graphics",
                "AMD Radeon R2 Graphics",
                "AMD Radeon HD 8280",
                "AMD Radeon HD 8240",
                "AMD Radeon HD 8210",
            };

            bool IsIntegratedGpu(string name)
            {
                return Devices.QueryDeviceType(name, VendorId.Amd) == DeviceType.Igp;
            }

            foreach (var name in KnownApuNames)
            {
                Assert.IsTrue(IsIntegratedGpu(name), $"expected '{name}' to match");
            }
        }

        [TestMethod("SelectGpuType should detect known AMD GPU")]
        public void TestDetectAmdGpu()
        {
            var isdGpu = Devices.QueryDeviceType("Radeon RX Vega 56", VendorId.Amd) ==
                DeviceType.Dgpu;
            Assert.IsTrue(isdGpu);
            // this one's from my old laptop
            isdGpu = Devices.QueryDeviceType("Radeon HD 5000 Series", VendorId.Amd) ==
                DeviceType.Dgpu;
            Assert.IsTrue(isdGpu);
        }
    }
}
