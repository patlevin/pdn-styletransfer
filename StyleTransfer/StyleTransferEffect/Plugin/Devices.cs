using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    // Type of the device
    enum DeviceType
    {
        // processor
        Cpu,
        // internal GPU (IGP/iGPU/APU)
        InternalGpu,
        // discrete GPU (PCIe slot)
        DiscreteGpu
    }

    // Manufacturer of the device
    enum DeviceManufacturer
    {
        // ??? (maybe ARM: Microsoft, Samsung, Qualcomm, ...)
        Unknown,
        // AMD
        Amd,
        // Intel
        Intel,
        // NVIDIA
        Nvidia
    }

    // Some basic properties of computation devices
    struct DeviceInfo
    {
        // Type of the device (CPU, IGP, GPU)
        public DeviceType Type;

        // DML device index
        public int DeviceId;

        // Display name of the device
        public string Name;

        // Manufacturer id
        public DeviceManufacturer Manufacturer;
    }

    // Computation device info provider
    static class Devices
    {
        // List all available compute devices
        public static IReadOnlyList<DeviceInfo> ComputeDevices
        {
            get
            {
                if (devices.Count == 0)
                {
                    devices.AddRange(QueryComputeDevices());
                }
                return devices.AsReadOnly();
            }
        }

        // Query devices from system
        private static IEnumerable<DeviceInfo> QueryComputeDevices()
        {
            var cpu = "";
            // select first CPU
            using (var searcher = new ManagementObjectSearcher("select Name from Win32_Processor"))
            {
                var e = searcher.Get().GetEnumerator();
                _ = e.MoveNext();
                cpu = (string)e.Current["Name"];
                yield return new DeviceInfo
                {
                    Type = DeviceType.Cpu,
                    DeviceId = -1,
                    Name = cpu,
                    Manufacturer = GetManufacturer(cpu)
                };
            }
            // append available GPUs
            using (var searcher = new ManagementObjectSearcher("select Name from Win32_VideoController"))
            {
                var collection = searcher.Get();
                var index = 0;
                foreach (var entry in collection)
                {
                    var name = (string)entry["Name"];

                    yield return new DeviceInfo
                    {
                        Type = index == 0 ?
                            SelectGpuType(name, cpu.StartsWith("Intel")) :
                            DeviceType.DiscreteGpu,
                        DeviceId = index++,
                        Name = name,
                        Manufacturer = GetManufacturer(name)
                    };
                }
            }
        }

        // try to guess whether the GPU is integrated (APU/iGPU/IGP) or discrete
        internal static DeviceType SelectGpuType(string name, bool isIntel)
        {
            string[] AMD_LIST = new string[]
            {
                @"^(AMD\s+)?Radeon(\(TM\))?\s+Graphics$",
                @"^(AMD\s+)?Radeon(\(TM\))?\s+HD\s+\d{4}D|G$",
                @"^(AMD\s+)?Radeon(\(TM\))?\s+HD\s+(840|833|82(1|4|8))0(\s+Graphics)?$",
                @"^(AMD\s+)?Radeon(\(TM\))?\s+HD\s+\d{4}(D|G)$",
                @"^(AMD\s+)?Radeon(\(TM\))?\s+R[2-7](\s+(Graphics|Series))?$",
                @"^(AMD\s+)?Radeon(\(TM\))?\s+(RX\s+)?Vega\s+(3|6|7|8|9|10|11)(\s+Graphics)?$",
                @"^R(5|7)$"
            };

            string[] INTEL_LIST = new string[]
            {
                @"^Intel\(R\)\s+HD\s+Graphics(\s+P?\d{3,4})?$",
                @"^Intel\(R\)\s+UHD\s+Graphics(\s+P?\d{3})?$",
                @"^Intel\(R\)\s+Iris(\s+(Pro|Plus))?\s+Graphics(\s+P?\d{3,4})?$",
                @"^Intel\(R\)\s+Iris\s+Xe\s+Graphics$",
            };

            // test for Intel­® IGP
            if (isIntel && INTEL_LIST.Any(pattern => Regex.IsMatch(name, pattern)))
            {
                return DeviceType.InternalGpu;
            }

            // test for AMD® IGP
            if (!isIntel && AMD_LIST.Any(pattern => Regex.IsMatch(name, pattern)))
            {
                return DeviceType.InternalGpu;
            }

            return DeviceType.DiscreteGpu;
        }

        // map device name to manufacturer id
        private static DeviceManufacturer GetManufacturer(string name)
        {
            var nameLower = name.ToLowerInvariant();
            if (nameLower.Contains("intel"))
            {
                return DeviceManufacturer.Intel;
            }

            if (nameLower.Contains("amd"))
            {
                return DeviceManufacturer.Amd;
            }

            if (nameLower.Contains("nvidia"))
            {
                return DeviceManufacturer.Nvidia;
            }

            return DeviceManufacturer.Unknown;
        }

        private static readonly List<DeviceInfo> devices = new List<DeviceInfo>();
    }
}
