\mainpage Welcome
This documentation contains documentation for %Treehopper's C# API. 

For other languages, visit [https://docs.treehopper.io/](https://docs.treehopper.io/).

# Features
%Treehopper's C# API is designed to support many different execution contexts. You can integrate %Treehopper into console applications that have 100% binary compatibility under Windows, macOS, and Linux.

%Treehopper also supports Windows 10 UWP app development, allowing you to write modern, lightweight, touch-friendly applications that run on Windows desktops and laptops, tablets, smartphones, the XBox, and even embedded devices like the Raspberry Pi.

%Treehopper's C# API integrates into Xamarin.Android (and consequently Xamarin.Forms) projects, providing support for Android deployments.

And, of course, there's full support for classic WPF and Windows Forms-style GUI applications.

## Libraries
In addition to the main API that allows you to manipulate pins and peripherals on %Treehopper, the C# API also includes an ever-growing library full of drivers for many different peripheral ICs, including IMUs and other sensors; GPIO expanders, DACs and ADCs; LED drivers, character and graphical displays; and motor drivers, rotary encoders, and other motion devices.

## Assemblies
%Treehopper's C# codebase is split across different assemblies:

- **%Treehopper**: the base library. Provides the core \link Treehopper.TreehopperUsb TreehopperUsb\endlink class for using \link Treehopper.Pin Pin\endlink, \link Treehopper.HardwarePwm PWM\endlink, \link Treehopper.HardwareI2C I2C\endlink, \link Treehopper.HardwareSpi SPI\endlink, and \link Treehopper.HardwareUart UART\endlink modules. Requires one of these connectors:
    - **%Treehopper.Desktop**: provides connectivity for traditional .NET Framework applications running on Windows, as well as Mono projects on macOS, or Linux. Also supports .NET Core 2.0 applications in Windows and Linux.
    - **%Treehopper.Android**: provides connectivity for Xamarin.Android projects.
    - **%Treehopper.Uwp**: provides connectivity for Windows 10 UWP (Windows Store) apps that can be deployed on all Windows 10 platforms.
- **%Treehopper.Libraries**: provides the %Treehopper.Libraries namespace, with support for more than 100 commonly-used ICs and peripherals.

# Documentation
If you'd like to browse the C# API to see how we approach things, we recommend checking out the [Modules](modules.html) page, which features a curated list of the most relevant \ref core-classes and \ref libraries organized in a logical manner.

If you're an advanced user that needs to track down a specific class, use the Search bar in the top, or visit the [Namespaces](namespaces.html) page.