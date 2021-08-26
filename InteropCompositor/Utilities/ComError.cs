using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static InteropCompositor.Interop.Win32;

namespace InteropCompositor.Utilities
{
    public static class ComError
    {
        [Guid("22F03340-547D-101B-8E65-08002B2BD119")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ICreateErrorInfo
        {
            [PreserveSig]
            HRESULT SetGUID([MarshalAs(UnmanagedType.LPStruct)] Guid rguid);

            [PreserveSig]
            HRESULT SetSource([MarshalAs(UnmanagedType.LPWStr)] string szSource);

            [PreserveSig]
            HRESULT SetDescription([MarshalAs(UnmanagedType.LPWStr)] string szDescription);

            [PreserveSig]
            HRESULT SetHelpFile([MarshalAs(UnmanagedType.LPWStr)] string szHelpFile);

            [PreserveSig]
            HRESULT SetHelpContext(int dwHelpContext);
        }

        [Guid("1CF2B120-547D-101B-8E65-08002B2BD119")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IErrorInfo
        {
            [PreserveSig]
            HRESULT GetGUID(out Guid pguid);

            [PreserveSig]
            HRESULT GetSource([MarshalAs(UnmanagedType.BStr)] out string pBstrSource);

            [PreserveSig]
            HRESULT GetDescription([MarshalAs(UnmanagedType.BStr)] out string pBstrDescription);

            [PreserveSig]
            HRESULT GetHelpFile([MarshalAs(UnmanagedType.BStr)] out string pBstrHelpFile);

            [PreserveSig]
            HRESULT GetHelpContext(out int pdwHelpContext);
        }

        public static void SetError(string description, [CallerMemberName] string source = null)
        {
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

            CreateErrorInfo(out ICreateErrorInfo errInfo).ThrowOnError();
            errInfo.SetDescription(description).ThrowOnError();
            if (source != null)
            {
                errInfo.SetSource(source).ThrowOnError();
            }

            IErrorInfo perrinfo = (IErrorInfo)errInfo;
            SetErrorInfo(0, perrinfo).ThrowOnError();
        }

        public static Exception GetError()
        {
            GetErrorInfo(0, out IErrorInfo perrinfo);
            if (perrinfo == null)
            {
                return null;
            }

            perrinfo.GetDescription(out string pBstrDescription);
            perrinfo.GetSource(out string pBstrSource);
            if (!string.IsNullOrWhiteSpace(pBstrSource))
            {
                pBstrDescription = ((pBstrDescription != null) ? (pBstrSource + ": " + pBstrDescription) : pBstrSource);
            }

            COMException ex = (!string.IsNullOrWhiteSpace(pBstrDescription)) ? new COMException(pBstrDescription) : new COMException();
            perrinfo.GetHelpFile(out string pBstrHelpFile);
            if (!string.IsNullOrWhiteSpace(pBstrHelpFile))
            {
                ex.HelpLink = pBstrHelpFile;
            }

            return ex;
        }

        [DllImport("oleaut32")]
        private static extern HRESULT GetErrorInfo(int dwReserved, out IErrorInfo perrinfo);

        [DllImport("oleaut32")]
        private static extern HRESULT SetErrorInfo(int dwReserved, IErrorInfo perrinfo);

        [DllImport("oleaut32")]
        private static extern HRESULT CreateErrorInfo(out ICreateErrorInfo errInfo);
    }
}
