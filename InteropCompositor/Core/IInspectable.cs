﻿using WinRT;
using System;
using System.Runtime.InteropServices;
using static InteropCompositor.Interop.Win32;

namespace InteropCompositor.Core
{
    [Guid("AF86E2E0-B12D-4c6a-9C5A-D7AA65101E90")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInspectable
    {
        [PreserveSig]
        HRESULT GetIids(out int iidCount, out IntPtr iids);

        [PreserveSig]
        HRESULT GetRuntimeClassName([MarshalAs(UnmanagedType.HString)] out string className);

        [PreserveSig]
        HRESULT GetTrustLevel(out TrustLevel trustLevel);
    }
}
