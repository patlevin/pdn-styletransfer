using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace PaintDotNet.Effects.ML.StyleTransfer.Dml
{
    /// <summary>
    /// Minimal HRESULT wrapper.
    /// </summary>
    struct Result
    {
        private readonly int code;

        /// <summary>
        /// Initialise result from HRESULT
        /// </summary>
        /// <param name="code">HRESULT code</param>
        public Result(int code)
        {
            this.code = code;
        }

        /// <summary>
        /// Get whether the wrapped result code denotes success.
        /// </summary>
        public bool Success => code >= 0;

        /// <summary>
        /// Get whether the wrapped result code denotes failure.
        /// </summary>
        public bool Failure => code < 0;

        /// <summary>
        /// Implicitly convert an HRESULT to a <see cref="Result"/> instance.
        /// </summary>
        /// <param name="code">Numerical HRESULT code</param>
        public static implicit operator Result(int code)
        {
            return new Result(code);
        }

        /// <summary>
        /// Wraps S_OK HRESULT code.
        /// </summary>
        public static readonly Result Ok = new Result(0);

        /// <summary>
        /// Wraps S_FALSE HRESULT code.
        /// </summary>
        public static readonly Result False = new Result(1);
    }

    /// <summary>
    /// Minimal managed C++ COM object wrapper.
    /// </summary>
    /// <remarks>
    /// The wrapper calls COM object methods using the C-style interface.
    /// This avoids messing with importing type libraries and generating
    /// unused code.
    /// </remarks>
    abstract class ComObject : IDisposable
    {
        // Interface UID
        static Guid uuid;

        /// <summary>
        /// Return the inteface UID
        /// </summary>
        /// <returns>Wrapped interface's UID</returns>
        public virtual ref Guid GetIID()
        {
            return ref uuid;
        }

        /// <summary>
        /// Initialise empty wrapper.
        /// </summary>
        protected ComObject()
        { }

        /// <summary>
        /// Get a native pointer to the interface for assignment.
        /// </summary>
        public ref IntPtr AssignPointer
        {
            get
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.Release(ptr);
                }

                return ref ptr;
            }
        }

        /// <summary>
        /// Get a native pointer to the wrapped interface.
        /// </summary>
        public IntPtr NativeInterface => ptr;

        /// <summary>
        /// Get whether the inteface pointer is valid.
        /// </summary>
        public bool IsValid => ptr != IntPtr.Zero;

        /// <summary>
        /// Wrapper for COM QueryInterface
        /// </summary>
        /// <typeparam name="T">Interface wrapper type</typeparam>
        /// <returns>Interface wrapper of the given type</returns>
        public T QueryInterface<T>() where T : ComObject, new()
        {
            var comObj = new T();
            return QueryInterface(ref comObj.GetIID(), out comObj.AssignPointer)
                .Success ? comObj : null;
        }

        /// <summary>
        /// Request  pointer to the specified interface from the COM object
        /// </summary>
        /// <param name="guid">Interface UID</param>
        /// <param name="instance">Native pointer to the interface</param>
        /// <returns>Resul of the call</returns>
        public Result QueryInterface(ref Guid guid, out IntPtr instance)
        {
            Contract.Requires(IsValid);
            return Marshal.QueryInterface(NativeInterface, ref guid, out instance);
        }

        /// <summary>
        /// Increment the reference counter on the wrapped interface
        /// </summary>
        /// <returns>New reference count</returns>
        public int AddRef()
        {
            Contract.Requires(IsValid);
            return Marshal.AddRef(ptr);
        }

        /// <summary>
        /// Decrement the reference couner on the wrapped interface
        /// </summary>
        /// <returns>New reference counter</returns>
        public int Release()
        {
            Contract.Requires(IsValid);
            return Marshal.Release(ptr);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ComObject()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsValid)
            {
                Release();
                ptr = IntPtr.Zero;
            }
        }

        // Return the C-style vtable of the wrapped C++ COM object
        IntPtr VTable => Marshal.ReadIntPtr(ptr);

        /// <summary>
        /// Return the native function pointer of the nth entry in the vtable
        /// </summary>
        /// <param name="functionIndex">VTABLE index of the function</param>
        /// <returns>Native pointer to the function</returns>
        protected IntPtr GetFunctionPointer(int functionIndex)
        {
            return Marshal.ReadIntPtr(VTable, functionIndex * IntPtr.Size);
        }

        /// <summary>
        /// Return a delegate for the nth function in the C++ COM object's vtable
        /// </summary>
        /// <typeparam name="T">Delegate type</typeparam>
        /// <param name="functionIndex">Index of the function in the object's vtable</param>
        /// <returns>Delegate for calling the function</returns>
        protected T GetFunction<T>(int functionIndex) where T : Delegate
        {
            var functionPointer = GetFunctionPointer(functionIndex);
            return Marshal.GetDelegateForFunctionPointer<T>(functionPointer);
        }

        private IntPtr ptr;
    }
}
