using System;
using Windows.UI.Composition;
using InteropCompositor.Core;
using System.Runtime.InteropServices;
using static InteropCompositor.Interop.Win32;

namespace InteropCompositor
{
#if NET
    [ComImport, Guid("A1BEA8BA-D726-4663-8129-6B5E7927FFA6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICompositionTarget : IInspectable
    {
        [PreserveSig]
        new HRESULT GetIids(out int iidCount, out IntPtr iids);

        [PreserveSig]
        new HRESULT GetRuntimeClassName([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(HStringMarshaler))] out string className);

        [PreserveSig]
        new HRESULT GetTrustLevel(out WinRT.TrustLevel trustLevel);

        Visual Root { get; set; }

    }

#else
    [ComImport, Guid("A1BEA8BA-D726-4663-8129-6B5E7927FFA6"), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface ICompositionTarget
    {
        Visual Root { get; set; }
    }
#endif
}
