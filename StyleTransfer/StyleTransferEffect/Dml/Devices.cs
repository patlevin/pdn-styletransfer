using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace PaintDotNet.Effects.ML.StyleTransfer.Dml
{
    /// <summary>
    /// Type of compute device.
    /// </summary>
    public enum DeviceType
    {
        Cpu,
        Igp,
        Dgpu
    }

    /// <summary>
    /// Vendor id.
    /// </summary>
    public enum VendorId
    {
        Other,
        Amd,
        Intel,
        Nvidia,
        Apple,
        Qualcomm,
        Microsoft
    }

    /// <summary>
    /// Compute-device information
    /// </summary>
    public struct DeviceInfo
    {
        /// <summary>
        /// DML device id.
        /// </summary>
        public int DeviceId;

        /// <summary>
        /// TYpe of the compute device.
        /// </summary>
        public DeviceType Type;

        /// <summary>
        /// Device description.
        /// </summary>
        public string Description;

        /// <summary>
        /// Device vendor name.
        /// </summary>
        public string VendorName;

        /// <summary>
        /// Device vendor id.
        /// </summary>
        public VendorId VendorId;

        /// <summary>
        /// Dedicated video memory in MiB.
        /// </summary>
        public int VideoMemoryInMB;

        /// <summary>
        /// Dedicated system memory in MiB.
        /// </summary>
        public int SystemMemoryInMB;

        /// <summary>
        /// Shared system memory in MiB.
        /// </summary>
        public int SharedMemoryInMB;
    }

    /// <summary>
    /// DML device list.
    /// </summary>
    public static class Devices
    {
        /// <summary>
        /// Return all available DML compute devices.
        /// </summary>
        /// <returns>List of available DML compute devices</returns>
        public static IReadOnlyList<DeviceInfo> GetComputeDevices()
        {
            if (deviceInfos == null)
            {
                deviceInfos = QueryDeviceInfos();
            }

            return deviceInfos;
        }

        private static DeviceInfo ConvertToDeviceInfo(int index, AdapterDesc desc)
        {
            var (vendorName, vendorId) = Lookup(desc.VendorId);
            return new DeviceInfo
            {
                DeviceId = index,
                Type = QueryDeviceType(desc.Description, vendorId),
                Description = desc.Description,
                VendorName = vendorName,
                VendorId = vendorId,
                VideoMemoryInMB = (int)(desc.DedicatedVideoMemory >> 20),
                SystemMemoryInMB = (int)(desc.DedicatedSystemMemory >> 20),
                SharedMemoryInMB = (int)(desc.SharedSystemMemory >> 20),
            };
        }


        private static IReadOnlyList<DeviceInfo> QueryDeviceInfos()
        {
            var factory = Factory.Create();
            if (factory == null)
            {
                return new DeviceInfo[0];
            }

            var list = new List<DeviceInfo> { QueryCpuDevice() };
            int index = 0;
            using (factory)
            {
                while (factory.EnumAdapters(index, out Adapter adapter).Success)
                {
                    using (adapter)
                    {
                        if (adapter.GetDesc(out AdapterDesc desc).Success &&
                            Adapter.IsSupported(desc.Flags))
                        {
                            list.Add(ConvertToDeviceInfo(index, desc));
                        }
                    }

                    ++index;
                }
            }

            return list.AsReadOnly();
        }

        private static DeviceInfo QueryCpuDevice()
        {
            const string Query = "select Name from Win32_Processor";
            using (var searcher = new ManagementObjectSearcher(Query))
            {
                var e = searcher.Get().GetEnumerator();
                _ = e.MoveNext();
                var cpuName = (string)e.Current["Name"];
                var (vendorName, vendorId) = Lookup(cpuName);
                return new DeviceInfo
                {
                    DeviceId = -1,
                    Type = DeviceType.Cpu,
                    Description = cpuName,
                    VendorName = vendorName,
                    VendorId = vendorId,
                };
            }
        }

        private static (string, VendorId) Lookup(uint vendorId)
        {
            // linear search is fine - we only call this a few times
            // (usually once or twice) and we don't search a giant list
            var result = PCI_ID.FirstOrDefault(MatchVendorId);

            return result.Item2 == null ? 
                ("Other", VendorId.Other) :
                (result.Item2, result.Item3);

            bool MatchVendorId((uint, string, VendorId) item)
            {
                return item.Item1 == vendorId;
            }
        }

        private static (string, VendorId) Lookup(string cpuName)
        {
            var result = PCI_ID.FirstOrDefault(MatchVendorName);

            return result.Item2 == null ?
                ("Other", VendorId.Other) :
                (result.Item2, result.Item3);

            bool MatchVendorName((uint, string, VendorId) item)
            {
                var (_, vendor, _) = item;
                return cpuName.Contains(vendor);
            }
        }

        internal static DeviceType QueryDeviceType(string desc, VendorId id)
        {
            switch (id)
            {
                case VendorId.Amd:
                    return gpuTestAMD.Any(IsIGP) ?
                        DeviceType.Igp : 
                        DeviceType.Dgpu;
                case VendorId.Intel:
                    return gpuTestIntel.Any(IsIGP) ?
                        DeviceType.Igp :
                        DeviceType.Dgpu;
                case VendorId.Apple:
                    return DeviceType.Igp;
                case VendorId.Qualcomm:
                    return DeviceType.Igp;
                default:
                    return DeviceType.Dgpu;
            }

            bool IsIGP(string pattern)
            {
                return Regex.IsMatch(desc, pattern);
            }
        }

        // some PCI IDs for likely GPU vendors
        private static readonly (uint, string, VendorId)[] PCI_ID =
        {
            (0x1002, "AMD", VendorId.Amd),
            (0x1022, "AMD", VendorId.Amd),
            (0x106b, "Apple", VendorId.Apple),
            (0x13b5, "ARM", VendorId.Other),
            (0x10de, "NVIDIA", VendorId.Nvidia),
            (0x1414, "Microsoft", VendorId.Microsoft),
            (0x17cb, "Qualcomm", VendorId.Qualcomm),
            (0x8086, "Intel", VendorId.Intel),
        };

        private static readonly string[] gpuTestAMD = new string[]
        {
            @"^(AMD\s+)?Radeon(\(TM\))?\s+Graphics$",
            @"^(AMD\s+)?Radeon(\(TM\))?\s+HD\s+\d{4}D|G$",
            @"^(AMD\s+)?Radeon(\(TM\))?\s+HD\s+(840|833|82(1|4|8))0(\s+Graphics)?$",
            @"^(AMD\s+)?Radeon(\(TM\))?\s+HD\s+\d{4}(D|G)$",
            @"^(AMD\s+)?Radeon(\(TM\))?\s+R[2-7](\s+(Graphics|Series))?$",
            @"^(AMD\s+)?Radeon(\(TM\))?\s+(RX\s+)?Vega\s+(3|6|7|8|9|10|11)(\s+Graphics)?$",
            @"^R(5|7)$"
        };

        private static readonly string[] gpuTestIntel = new string[]
        {
            @"^Intel\(R\)\s+HD\s+Graphics(\s+P?\d{3,4})?$",
            @"^Intel\(R\)\s+UHD\s+Graphics(\s+P?\d{3})?$",
            @"^Intel\(R\)\s+Iris(\s+(Pro|Plus))?\s+Graphics(\s+P?\d{3,4})?$",
            @"^Intel\(R\)\s+Iris\s+Xe\s+Graphics$",
        };

        private static IReadOnlyList<DeviceInfo> deviceInfos;
    }
}
