\page getting-started Getting Started
We have Getting Started guides for the following platforms:
- \subpage windows
- \subpage mac
- \subpage linux
- \subpage android

# Concepts
Before you dive into the C# API, you should familiarize yourself with some core concepts found in the API, along with the overall API philosophy. This will help you anticipate how to interact with the API so that you don't have to constantly consult the docs directly.

## Property Oriented
C# has native support for properties, thus %Treehopper property names directly translate. Most classes implement INotifyPropertyChanged; this allows XAML-based GUIs targetting WPF, UWP, or Xamarin.Forms to directly bind to peripheral properties.

## Async/Await
To keep GUI applications running smoothly without the developer having to worry about explicitly creating background threads, the %Treehopper C# API provides asynchronous APIs for many tasks.

## Name and Serial Number
Each %Treehopper board has a serial number and a name. Both of these properties can be set by the user. Note that these properties correspond to the ProductName and SerialNumber that are part of the USB specification, which means they'll be visible across your operating system.

## Board Discovery (ConnectionService)
All %Treehopper C# connectivity APIs provide a static instance of the Treehopper.ConnectionService class that can be used to retrieve instances to attached %Treehopper boards.

You can use multiple %Treehopper boards simultaneously from a single application. Access these boards from the Treehopper.ConnectionService.Instance.Boards collection.

## Simultaneous Access
Only one application can connect to a %Treehopper board at a time. This has some important repercussions:
 - <b>Avoid creating instances of Treehopper.ConnectionService; use the static Treehopper.ConnectionService.Instance property it provides for all access</b>. If you want to share a board between different areas of your code (for example, between decoupled ViewModels in a MVVM-style application), you must share the board object (or the ConnectionService instance that can be used to retrieve the board object). Do not create instances of ConnectionService in each module and attempt to access the boards concurrently; this will fail.
 - <b>When possible, Treehopper.ConnectionService will query the OS --- not the device directly --- about its name and serial number</b>. This allows an application to scan all the boards attached to a computer; even if the boards are in use by other applications. Unfortunately, this is not currently supported in Linux.

 # Xamarin.Forms Support
 %Treehopper has good support for Xamarin.Forms and any other cross-platform system that uses shared projects. While the ConnectionService class is platform-specific, each platform's implementation lives in the same %Treehopper namespace, so they overlay seamlessly in code-sharing projects.