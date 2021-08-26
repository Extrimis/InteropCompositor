using System;
using System.Windows;
using System.Numerics;
using InteropCompositor;
using System.Windows.Interop;
using Windows.UI.Composition;
using System.Runtime.InteropServices;

namespace Sample
{
    public partial class MainWindow : Window
    {
        Compositor compositor;
        private readonly object dispatcherQueue;
        private ICompositionTarget? compositionTarget;
        private ICompositorDesktopInterop interop;

        public MainWindow()
        {
            InitializeComponent();
            dispatcherQueue = InitializeCoreDispatcher();
            compositor = new Compositor();
            interop = compositor.TryAs<ICompositorDesktopInterop>();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).EnsureHandle();
            interop.CreateDesktopWindowTarget(hwnd, true, out var target).ThrowOnError();
            compositionTarget = (ICompositionTarget)target;
            enableHostBackdropBrush(hwnd);

            SpriteVisual rootVisual = compositor.CreateSpriteVisual();
            int width = (int)SystemParameters.PrimaryScreenWidth;
            int height = (int)SystemParameters.PrimaryScreenHeight;
            rootVisual.Size = new Vector2(width, height);
            rootVisual.Brush = compositor.CreateHostBackdropBrush();
            compositionTarget.Root = rootVisual;
        }

        private object InitializeCoreDispatcher()
        {
            DispatcherQueueOptions options = new DispatcherQueueOptions
            {
                apartmentType = DISPATCHERQUEUE_THREAD_APARTMENTTYPE.DQTAT_COM_STA,
                threadType = DISPATCHERQUEUE_THREAD_TYPE.DQTYPE_THREAD_CURRENT,
                dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions))
            };

            var hresult = CreateDispatcherQueueController(options, out object queue);
            if (hresult != 0)
            {
                Marshal.ThrowExceptionForHR(hresult);
            }
            return queue;
        }

        private void enableHostBackdropBrush(IntPtr hwnd)
        {
            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_USE_HOST_BACKDROP;
            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(hwnd, ref data);
            Marshal.FreeHGlobal(accentPtr);
        }

        internal enum DISPATCHERQUEUE_THREAD_APARTMENTTYPE
        {
            DQTAT_COM_NONE = 0,
            DQTAT_COM_ASTA = 1,
            DQTAT_COM_STA = 2
        };

        internal enum DISPATCHERQUEUE_THREAD_TYPE
        {
            DQTYPE_THREAD_DEDICATED = 1,
            DQTYPE_THREAD_CURRENT = 2,
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct DispatcherQueueOptions
        {
            public int dwSize;
            [MarshalAs(UnmanagedType.I4)]
            public DISPATCHERQUEUE_THREAD_TYPE threadType;
            [MarshalAs(UnmanagedType.I4)]
            public DISPATCHERQUEUE_THREAD_APARTMENTTYPE apartmentType;
        };

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
            ACCENT_USE_HOST_BACKDROP = 5,
            ACCENT_INVALID_STATE = 6,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public uint AccentFlags;
            public uint GradientColor;
            public uint AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }

        [DllImport("coremessaging.dll")]
        internal static extern int CreateDispatcherQueueController(DispatcherQueueOptions options, [MarshalAs(UnmanagedType.IUnknown)] out object dispatcherQueueController);

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
    }
}
