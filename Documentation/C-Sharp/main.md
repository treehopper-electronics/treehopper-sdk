\mainpage Welcome

# Introduction {#intro}
This documentation contains all C#-specific information for interfacing with %Treehopper. For hardware documentation, or for documentation for other languages, visit <a href="https://docs.treehopper.io/">https://docs.treehopper.io/</a>.


## Features {#features}
%Treehopper's C# API is designed to support many different execution contexts; you can integrate it into simple console applications that have 100% binary compatibility under Windows, macOS, Linux, and other UNIX-like operating systems. 

%Treehopper also supports Windows 10 UWP app development, allowing you to write modern, lightweight, touch-friendly applications that run on Windows desktops and laptops, tablets, smartphones, the XBox, and even embedded IoT devices. 

There's C# support for Android deployments as well (provided by the Xamarin platform). And, of course, there's full support for traditional WPF and Windows Forms-style GUI applications (we even include some helper components for building loosely-coupled MVVM-friendly applications).

### Libraries {#libraries}
In addition to the main API that allows you to manipulate and sample pins on the %Treehopper, the C# API also includes an ever-growing library full of drivers for many different peripheral ICs, including IMUs and other sensors; GPIO expanders, DACs and ADCs; LED drivers, character and graphical displays; and motor drivers, rotary encoders, and other motion devices.

### Assemblies {#assemblies}
%Treehopper's C# codebase is split across different assemblies:

- Treehopper: the base library. Provides the core TreehopperUsb class for GPIO, PWM, I2C, SPI, and base interface support. Requires one of these connectors:
    - Treehopper.Desktop: provides connectivity for traditional console or non-UWP GUI applications running on Windows, macOS, or Linux.
    - Treehopper.Android: provides connectivity for Xamarin.Android projects.
    - Treehopper.Uwp: provides connectivity for UWP (Windows Store) apps that can be deployed on all Windows 10 platforms.
- Treehopper.Libraries: provides support for more than 100 commonly-used ICs and peripherals.

### .NET Core Support
Treehopper, Treehopper.Desktop, and Treehopper.Libraries APIs are built in .NET Standard 2.0 whenever possible. This allows %Treehopper apps to be built using the new, lightweight, open-source, cross-platform .NET Core tooling --- in addition to classic Mono Project.

# Concepts
Before you dive into the C# API, you should familiarize yourself with some core concepts found in the API, along with the overall API philosophy. This will help you anticipate how to interact with the API so that you don't have to constantly consult the docs directly.

## Property Oriented
C# has native support for properties, thus %Treehopper property names directly translate. Most classes implement INotifyPropertyChanged; this allows XAML-based GUIs targetting WPF, UWP, or Xamarin.Forms to directly bind to properties.

## Async/Await
To keep GUI applications running smoothly without the developer having to worry about explicitly creating background threads, the %Treehopper C# API provides asynchronous APIs for many tasks.

## Name and Serial Number
Each %Treehopper board has a serial number and a name. Both of these properties can be set by the user. Note that these properties correspond to the ProductName and SerialNumber that are part of the USB specification, which means they'll be visible across your operating system. Note that Windows does not refresh the name of the device in Device Manager.

## Board Discovery (ConnectionService)
All %Treehopper connectivity APIs provide a static instance of the Treehopper.ConnectionService class that can be used to retrieve instances to attached %Treehopper boards.

You can use multiple %Treehopper boards simultaneously from a single application.

## Simultaneous Access
Only one connection can be made to a %Treehopper board. This has some important repercussions:
 - <b>Avoid creating instances of Treehopper.ConnectionService; use the static Treehopper.ConnectionService.Instance property it provides for all access</b>. If you want to share a board between different areas of your code (for example, between decoupled ViewModels in a MVVM-style application), you must share the board object (or the ConnectionService instance that can be used to retrieve the board object). Do not create instances of ConnectionService in each module and attempt to access the boards concurrently; this will fail.
 - <b>When possible, Treehopper.ConnectionService will query the OS --- not the device directly --- about its name and serial number</b>. This allows an application to scan all the boards attached to a computer; even if the boards are in use by other applications. Unfortunately, this is not currently supported in Linux.

 # Xamarin.Forms Support
 %Treehopper's user-facing platform-specific calls reside in Treehopper.ConnectionService. To make Xamarin.Forms and other code-sharing projects effortless, all ConnectionService classes live in the same %Treehopper namespace, so they overlay effortlessly.