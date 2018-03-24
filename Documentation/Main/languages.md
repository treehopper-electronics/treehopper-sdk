\page languages Supported Languages

Treehopper has native APIs for C#, Python, Java, and C++. Each language API has advantages and disadvantages --- especially for novice programmers. This page outlines architectural and functional differences between each language.

*This page will be updated as new languages and new API features are integrated.*

# C♯
C# is a modern, dynamic, strongly-typed language built by Microsoft as the premier language for their .NET platform. 

The C# API is the most developed API for Treehopper. Along with Java, it has support for almost every major platform: classic Windows console and desktop, Windows 10 UWP (Windows Store), Linux and macOS (through Mono), and Android (through Xamarin). 

Its UWP support, in particular, means your Treehopper apps can target Windows desktops, laptops, tablets, smartphones, XBox One, and Windows 10 IoT Core devices.

The C# API is the most comprehensive --- while all languages provide the same core functionality, we tend to build out C# peripheral libraries first.

The design goal for the C# API is to target modern, GUI-centric application development. While it is perfectly suitable for basic console apps, the API is more verbose than, say, the Python one.

For example, there's a ConnectionService manager singleton that you must interact with to obtain board references, and many calls require async/await-style programming. This can be a challenging concept for beginners to master, and depending on the execution environment, deadlocks can occur if the calls aren't implemented properly.

Treehopper's C# API has excellent cross-platform support, but until Xamarin.Forms gets better macOS and GTK# backends, writing cross-platform GUIs is most easily done using the Java API.

## When to use the C# API
 - When you're building Windows GUI applications or Xamarin Android, Mac, or Xamarin.Forms apps.
 - When you need support for a peripheral not found in the libraries packages for other language APIs.

# Python
Python is a high-level dynamically-typed interpreted language with good multi-platform open-source support, and an emphasize on conciseness and readability. Because of its interpreted nature (and the success of IPython and Jupyter Notebook), Python is widely used for general-purpose automation, problem-solving, experimentation, and scientific computing.

Treehopper's Python API may be the newest addition to the support list, but it's being developed so rapidly that it will quickly catch up to (and outpace) other Treehopper language APIs.

The design goal of the Python API is to provide an extremely concise, readable interface that reimagines Treehopper in an unabashed Pythonic manner. This comes with a few disadvantages --- the biggest being the lack of hot-plug support while the library is running. This may be integrated later on, but would require work on upstream LibUSB and PyUSB, along with fleshing out several changes in the core API.

The biggest advantage of the Python API when compared to other languages is more just the advantages of Python itself. Its dynamic nature (and interactive shell support) makes it easier than ever to quickly start a project.

Consequently, the Python API --- combined with an IPython interactive shell --- turns your Treehopper into a useful interactive debugging / diagnostic tool that can be used to probe around I2C devices, SPI memory, and anything else that uses electricity.

It's also a great API to get familiar with Treehopper, since Python is easy to install in every operating system, and all you need is a text editor and console window to run your Python code.

## When to use the Python API
 - When you're integrating Treehopper into existing Python scripts.
 - When you're quickly prototyping or debugging --- or you're looking for a simple starting point.

# Java
Java is a high-level strongly-typed object-oriented language that became a popular cross-platform desktop app environment, but is now most notable as the primary language used for Android development.

Treehopper's Java API is second only to C# in completeness; it has full support for all core functions, and also has many peripheral drivers in the io.treehopper.libraries package.

The Java API has two connectivity libraries --- one that supports Android (through the UsbHost and UsbDevice APIs), and one that supports "everything else" --- desktop and laptop deployment in Windows, macOS, and Linux. The Android connectivity package is thoroughly tested, and has been deployed in commercial projects. It provides full hotplug support so your activities can detect board connect/disconnect events.

The desktop connectivity package is a late addition to fill a large support gap with minimal work; it is a simpler API that calls into usb4java (which is used as a LibUSB wrapper), instead of implementing native OS calls directly (like the C# API does). Consequently, any sort of compatibility issues with LibUSB or usb4java will propagate to the Java desktop package. The lack of hotplug support on Windows is probably the biggest outstanding issue.

## When to use the Java API
 - When you're building Android apps using a traditional Android development path.
 - When you're creating cross-platform GUI apps that need to run on Windows, macOS, and Linux with 100% binary compatibility.

# C++
C++ is a compiled, strongly-typed, object-oriented language used in high-performance applications.

Treehopper has a C++ API that covers core functionality, along with some preliminary library support. C++ is an advanced language that has many subtle memory-management differences from the other language APIs on this page.

The C++ API should be considered for advanced users only. We don't provide pre-compiled packages; you'll have to build the library from our version-controlled source. We use [CMake](https://cmake.org/), which allows the source code to be built with native tools on all operating systems.

Treehopper has connectivity support for Windows (through WinUSB), Linux (through LibUSB), and macOS (through IOKit). We do not support Android NDK development.

## When to use the C++ API
 - When you need to integrate Treehopper support into existing C++ applications.
 - When you're targeting extremely resource-constrained devices (such as consumer routers).
 
 In the second case, especially, the C++ API will likely be your only solution, as these devices often have less than 64 MB of RAM, run at 400 MHz or less, and might only have a few megabytes of storage to store a compressed filesystem.


# Other Environments
While we provide support for four popular languages, there are many other environments and languages out there. Some of these can directly use libraries we already have; some require porting. 

## MATLAB
MATLAB is a popular programming environment used in scientific computing. While Treehopper originally was scheduled to have a native MATLAB API, this effort was abandoned, as MATLAB has excellent support for interfacing with .NET and Java --- and a minimal MEX shim can be created to interface with C++, too.

If you're on Windows, we recommend you simply [load the .NET assemblies into MATLAB](https://www.mathworks.com/help/matlab/ref/net.addassembly.html) and [call into them directly](https://www.mathworks.com/help/matlab/matlab_external/using-a-net-object.html).

Similarly, on Linux or macOS, you can accomplish the same with the Java interop support built into MATLAB by loading the Treehopper JAR files and calling into them directly. You can use the JAR on Windows, too, but the .NET API currently provides more functionality than the Java API, so we recommend this approach on Windows.

We provide MATLAB examples in our GitHub repo to illustrate both .NET and Java API access.

## Porting to a new language
There are lots of wonderful languages and environments out there that Treehopper doesn't support yet. We'd love to write apps that target Treehopper in JavaScript, Rust, Go, Swift, node.js, Electron, Cordova, and Chrome, and we'd love your help!

We have emerging documentation of the Treehopper protocol on the [GitHub Wiki for the treehopper-sdk repo](https://github.com/treehopper-electronics/treehopper-sdk/wiki). If you're a developer interested in bringing Treehopper to a new language, framework, or platform, get in touch with us [on the forums](https://community.treehopper.io) or [on our Gitter](https://gitter.im/treehopper-electronics/Lobby) and we'd love to chat about your efforts.

The underlying protocol is simple --- if you have access to a USB library that abstracts the OS-specific busywork, you should be able to port most of the core functions over in a day or two.