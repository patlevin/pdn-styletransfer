using System;
using System.Runtime.InteropServices;

namespace PaintDotNet.Effects.ML.StyleTransfer.Dml
{
    /// <summary>
    /// Minimal wrapper around IDXGIFactory1
    /// see https://docs.microsoft.com/en-us/windows/win32/api/dxgi/nn-dxgi-idxgifactory1
    /// </summary>
    class Factory : ComObject
    {
        static Guid uuid = new Guid("770aae78-f26f-4dba-a829-253c83d1b387");

        public override ref Guid GetIID()
        {
            return ref uuid;
        }

        /// <summary>
        /// Return an instance of the IDXGIFactory1 wrapper.
        /// </summary>
        /// <returns>Factory instance;<c>null</c> on failure.</returns>
        public static Factory Create()
        {
            var factory = new Factory();
            Result result = CreateDXGIFactory1(ref uuid, out factory.AssignPointer);
            return result.Failure ? null : factory;
        }

        /// <summary>
        /// Query a DXGI adapter by index
        /// see https://docs.microsoft.com/en-us/windows/win32/api/dxgi/nf-dxgi-idxgifactory1-enumadapters1
        /// </summary>
        /// <param name="index">Adapter index</param>
        /// <param name="adapter">COM wrapper for IDXGIAdapter1</param>
        /// <returns>Result of the function; <see cref="Result.Ok"/> on success</returns>
        public Result EnumAdapters(int index, out Adapter adapter)
        {
            if (enumAdapters == null)
            {
                enumAdapters = GetFunction<EnumAdapters1>(functionIndex: 12);
            }

            adapter = new Adapter();
            return enumAdapters(NativeInterface, index, out adapter.AssignPointer);
        }

        private delegate int EnumAdapters1(IntPtr self, int index, out IntPtr adapter);

        private EnumAdapters1 enumAdapters;

        [DllImport("dxgi")]
        private static extern int CreateDXGIFactory1(ref Guid riid, out IntPtr ppFactory);
    }
}
