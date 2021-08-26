using InteropCompositor.Core;
using System;
using System.Runtime.InteropServices;
using static InteropCompositor.Interop.Win32;

namespace InteropCompositor
{
#if NET
    [ComImport, Guid("6329D6CA-3366-490E-9DB3-25312929AC51"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDesktopWindowTarget : IInspectable
    {
        [PreserveSig]
        new HRESULT GetIids(out int iidCount, out IntPtr iids);

        [PreserveSig]
        new HRESULT GetRuntimeClassName([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(HStringMarshaler))] out string className);

        [PreserveSig]
        new HRESULT GetTrustLevel(out WinRT.TrustLevel trustLevel);

        [PreserveSig]
        HRESULT get_IsTopmost(out bool value);
    }
#else
    [ComImport, Guid("6329D6CA-3366-490E-9DB3-25312929AC51"), InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    public interface IDesktopWindowTarget
    {
        [PreserveSig]
        HRESULT get_IsTopmost(out bool value);
    }
#endif
}
