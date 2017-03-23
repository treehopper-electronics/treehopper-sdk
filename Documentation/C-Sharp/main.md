\mainpage Welcome

# Introduction {#intro}

This documentation contains all C#-specific information for interfacing with Treehopper. For hardware documentation, or for documentation for other languages, visit <a href="http://treehopper.io/Documentation">http://treehopper.io/documentation</a>.


## Features {#features}
Treehopper's C# API is designed to support many different execution contexts; you can integrate it into simple console applications that have 100% binary compatibility under Windows, macOS, Linux, and other UNIX-like operating systems. Treehopper also supports Windows 10 UWP app development, allowing you to write modern, lightweight, touch-friendly applications that run on Windows desktops and laptops, tablets, smartphones, the XBox, and even embedded IoT devices. There's C# support for Android deployments as well (provided by the Xamarin platform). And, of course, there's full support for traditional WPF and Windows Forms-style GUI applications (we even include some helper components for building loosely-coupled MVVM-friendly applications).

## Libraries {#libraries}
In addition to the main API that allows you to manipulate and sample pins on the Treehopper, the C# API also includes an ever-growing library full of drivers for many different peripheral ICs, including IMUs and other sensors; GPIO expanders, DACs and ADCs; LED drivers, character and graphical displays; and motor drivers, rotary encoders, and other motion devices.

## Assemblies {#assemblies}
Because so many different execution contexts are supported, Treehopper's C# codebase has to be split across different assemblies. All of these are available on NuGet, or you can always clone the Treehopper SDK repo and build these assemblies from source. Here are the assemblies to keep in mind:

- Treehopper.dll: the base library. Provides GPIO, PWM, I2C, SPI, and base interface support. Exposes the Treehopper namespace. Requires one of these connectors:
    - Treehopper.Desktop.dll: provides platform-agnostic connectivity for traditional console or desktop applications running on Windows, macOS, or Linux. Exposes the Treehopper.Desktop namespace. On Windows, we call into native WinUSB functions provided by Windows; on other platforms, we call into LibUSB functions. If you're running in a non-Windows environment, make sure you have LibUSB runtime libraries installed (this is less critical on Linux, but macOS users will need to install them via brew).
    - Treehopper.Android.dll: provides connectivity for C# Xamarin projects that target Android. Exposes the Treehopper.Android namespace.
    - Treehopper.Uwp.dll: provides connectivity for UWP (Windows Store) apps that can be deployed on all Windows 10 platforms. Exposes the Treehopper.Uwp namespace. This library also provides MVVM-friendly helpers to abstract away some of the tedious state-handling of Treehopper connectivity.
- Treehopper.Libraries.dll: provides support for more than 100 commonly-used ICs and peripherals.

# Concepts
Before you dive into the C# API, you should familiarize yourself with some core concepts found in the API, along with the overall API philosophy. This will help you anticipate how to interact with the API so that you don't have to constantly consult the docs directly.

## Name and Serial Number
Each Treehopper board has a serial number and a name. Both of these properties can be set by the user. Note that these properties correspond to the ProductName and SerialNumber that are part of the USB specification, which means they'll be visible across your operating system. Note that Windows does not refresh the name of the device in Device Manager.

## Board Discovery (ConnectionService)
Most Treehopper language APIs provide a static instance of the ConnectionService class that can be used to discover attached Treehopper boards. Depending on the language you're using, this class might provide events that you can subscribe to so you can receive notifications when boards are plugged and unplugged. Treehopper's core API is designed from the ground up to support an arbitrarily large number of connected devices, so most language APIs provide a dynamic list of attached boards. Some language APIs allow you to query the ConnectionService instance to discover boards that match certain criteria (i.e., have a specific serial number or name).

## Simultaneous Access
Only one connection can be made to a Treehopper board, and the library supports connecting to as many Treehopper boards as you want. This has some important repercussions:
 - <b>Avoid creating instances of ConnectionService; use the static instance property it provides for all access</b>. If you want to share a board between different areas of your code (for example, between decoupled ViewModels in a MVVM-style application), you must share the board object (or the ConnectionService instance that can be used to retrieve the board object). Do not create instances of ConnectionService in each module and attempt to access the boards concurrently; this will fail.
 - <b>When possible, ConnectionService will query the OS --- not the device directly --- about its name and serial number</b>. This allows an applications to scan all the boards attached to a computer; even if the boards are connected to other applications.