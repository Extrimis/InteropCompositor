# Interop Compositor
With the release of .NET5 Preview 8, Microsoft introduced a streamlined way for .NET developers to access the set of Windows Runtime (WinRT) APIs that are shipped as part of Windows..NET5 applications can now access Windows APIs through a new set of [Target Framework Monikers (TFMs)](https://docs.microsoft.com/en-us/dotnet/standard/frameworks), which have been extended to support platform specific APIs.

This change means that applications targeting .NET5 no longer need to reference the `Microsoft.Windows.SDK.Contracts` NuGet package.
But in Target Framework Moniker Using `Compositor` object to get `ICompositorDesktopInterop` throws exception
>System.PlatformNotSupportedException: 'Marshalling as IInspectable is not supported in the .NET runtime.'

To overcome this issue you can use **InteropCompositor**.

## Install
*NuGet Package*
```
Install-Package InteropCompositor
```
[https://nuget.org/packages/InteropCompositor](https://nuget.org/packages/InteropCompositor)

## Usage

Set the TargetFramework in the project file
```XML
<TargetFramework>net5.0-windows10.0.19041.0</TargetFramework>
```
and in your code use
```C#
Compositor compositor = new Compositor();
ICompositorDesktopInterop interop = compositor.TryAs<ICompositorDesktopInterop>();
interop.CreateDesktopWindowTarget(hwnd, true, out var target).ThrowOnError();
ICompositionTarget compositionTarget = (ICompositionTarget)target;
```
*Note: Don't forget to create `DispatcherQueueController` on MainWindow Constructor.*

## Credits :medal_sports:
[Simon Mourier](https://github.com/smourier) and his Repository [DirectN](https://github.com/smourier/DirectN/tree/master/DirectN)
