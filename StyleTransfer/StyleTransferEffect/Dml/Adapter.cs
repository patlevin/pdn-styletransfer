using System;
using System.Runtime.InteropServices;

namespace PaintDotNet.Effects.ML.StyleTransfer.Dml
{
    /// <summary>
    /// Local UID - identifier unique to the local machine.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct LUID
    {
        public uint LowPart;
        public int HiPart;
    }

    /// <summary>
    /// See https://docs.microsoft.com/en-us/windows/win32/api/dxgi/ns-dxgi-dxgi_adapter_desc1
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct AdapterDesc
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Description;
        public uint VendorId;
        public uint DeviceId;
        public uint SubSystemId;
        public uint Revision;
        public ulong DedicatedVideoMemory;
        public ulong DedicatedSystemMemory;
        public ulong SharedSystemMemory;
        public LUID AdapterLuid;
        public uint Flags;
    }

    /// <summary>
    /// Minimal wrapper for IDXGIAdapter1
    /// see https://docs.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgiadapter1
    /// </summary>
    class Adapter : ComObject
    {
        // Remote flag
        const uint REMOTE = 0x00000001;
        // Software renderer flag
        const uint SOFTWARE = REMOTE + 0x1u;
        // IDXGIAdapter1 IID
        static Guid uuid = new Guid("29038f61-3839-4626-91fd-086879011a05");

        public override ref Guid GetIID()
        {
            return ref uuid;
        }

        /// <summary>
        /// Return whether the given flags indicate a supported compute device
        /// </summary>
        /// <param name="flags">Flags from <see cref="AdapterDesc"/></param>
        /// <returns><c>true</c>, iff an adapter with these flags can be used with DML</returns>
        public static bool IsSupported(uint flags)
        {
            return (flags & (REMOTE | SOFTWARE)) == 0;
        }

        /// <summary>
        /// Get the adapter description.
        /// </summary>
        /// <param name="desc">Adapter description</param>
        /// <returns>Result of the query.</returns>
        public Result GetDesc(out AdapterDesc desc)
        {
            if (getDesc1 == null)
            {
                getDesc1 = GetFunction<GetDesc1>(functionIndex: 10);
            }

            return getDesc1(NativeInterface, out desc);
        }

        private delegate int GetDesc1(IntPtr self, out AdapterDesc desc);
        GetDesc1 getDesc1;
    }
}
