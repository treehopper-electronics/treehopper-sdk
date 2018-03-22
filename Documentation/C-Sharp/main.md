\mainpage Welcome
This documentation contains all C#-specific information for interfacing with %Treehopper. For hardware documentation, or for documentation for other languages, visit <a href="https://docs.treehopper.io/">https://docs.treehopper.io/</a>.


## Features {#features}
%Treehopper's C# API is designed to support many different execution contexts. 

You can integrate %Treehopper into console applications that have 100% binary compatibility under Windows, macOS, and Linux.

%Treehopper also supports Windows 10 UWP app development, allowing you to write modern, lightweight, touch-friendly applications that run on Windows desktops and laptops, tablets, smartphones, the XBox, and even embedded devices like the Raspberry Pi.

There's C# support for Android deployments as well (provided by the Xamarin platform).

And, of course, there's full support for traditional WPF and Windows Forms-style GUI applications.

### Libraries {#libraries}
In addition to the main API that allows you to manipulate and sample pins on the %Treehopper, the C# API also includes an ever-growing library full of drivers for many different peripheral ICs, including IMUs and other sensors; GPIO expanders, DACs and ADCs; LED drivers, character and graphical displays; and motor drivers, rotary encoders, and other motion devices.

### Assemblies {#assemblies}
%Treehopper's C# codebase is split across different assemblies:

- **%Treehopper**: the base library. Provides the core \link Treehopper.TreehopperUsb TreehopperUsb\endlink class for \link Treehopper.TreehopperUsb.Pin Pin\endlink, PWM, I2C, SPI, and base interface support. Requires one of these connectors:
    - **%Treehopper.Desktop**: provides connectivity for traditional console or non-UWP GUI applications running on Windows, macOS, or Linux.
    - **%Treehopper.Android**: provides connectivity for Xamarin.Android projects.
    - **%Treehopper.Uwp**: provides connectivity for UWP (Windows Store) apps that can be deployed on all Windows 10 platforms.
- **%Treehopper.Libraries**: provides support for more than 100 commonly-used ICs and peripherals.

### .NET Core Support
**%Treehopper**, **%Treehopper.Desktop**, and **%Treehopper.Libraries** assemblies are built in .NET Standard 2.0. This allows %Treehopper apps to be built using the new, lightweight, open-source, cross-platform .NET Core tooling --- in addition to classic .NET Framework and Mono Project runtimes.