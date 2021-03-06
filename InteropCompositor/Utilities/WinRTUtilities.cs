using WinRT;
using System;
using InteropCompositor.Core;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;
using static InteropCompositor.Interop.Win32;

namespace InteropCompositor
{
    public static class WinRTUtilities
    {
        private static readonly ConcurrentDictionary<ushort, bool> _isApiContractAvailable = new ConcurrentDictionary<ushort, bool>();

        [DllImport("combase")]
        private static extern HRESULT RoGetActivationFactory([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(HStringMarshaler))] string activatableClassId, [MarshalAs(UnmanagedType.LPStruct)] Guid iid, [MarshalAs(UnmanagedType.IUnknown)] out object factory);

        public static T GetActivationFactory<T>(string activatableClassId, bool throwOnError = true) => (T)GetActivationFactory(activatableClassId, typeof(T).GUID, throwOnError);

        public static object GetActivationFactory(string activatableClassId, Guid iid, bool throwOnError = true)
        {
            if (activatableClassId == null)
            {
                throw new ArgumentNullException(nameof(activatableClassId));
            }

            RoGetActivationFactory(activatableClassId, iid, out var comp).ThrowOnError(throwOnError);
            return comp;
        }

        public static bool Is19H1OrHigher => IsApiContractAvailable(8);

        public static bool IsApiContractAvailable(ushort version)
        {
            if (_isApiContractAvailable.TryGetValue(version, out var available))
            {
                return available;
            }

            available = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", version);
            _isApiContractAvailable[version] = available;
            return available;
        }

        public static T WinRTCast<T>(this object obj, bool throwOnError = true) where T : class
        {
            if (obj == null)
            {
                return default;
            }

            if (throwOnError)
            {
                #if NET
                var unkt = Marshal.GetIUnknownForObject(obj);
                return MarshalInterface<T>.FromAbi(unkt);
                #else
                return (T)obj;
                #endif
            }

            #if NET
            var unk = Marshal.GetIUnknownForObject(obj);
            if (unk == IntPtr.Zero)
            {
                return default;
            }

            try
            {
                return MarshalInterface<T>.FromAbi(unk);
            }
            catch
            {
                return default;
            }
            #else
            return obj as T;
            #endif
        }

        public static T TryAs<T>(this object obj, bool throwOnError = true) where T : class
        {
            if (obj == null)
            {
                return default;
            }

            if (throwOnError)
            {
                #if NET
                return obj.As<T>();
                #else
                return (T)obj;
                #endif
            }

            #if NET
            try
            {
                return obj.As<T>();
            }
            catch
            {
                return default;
            }
            #else
            return obj as T;
            #endif
        }
    }
}
