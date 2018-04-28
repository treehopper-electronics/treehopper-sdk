\page getting-started Getting Started

We have Getting Started guides for the following platforms:
- \subpage windows
- \subpage mac
- \subpage linux

# Concepts
Before you dive into the C++ API, you should familiarize yourself with some core concepts found in the API, along with the overall API philosophy. This will help you anticipate how to interact with the API so that you don't have to constantly consult the docs directly.

## Property Oriented
%Treehopper property names directly translate to overloaded C++ class method. Leave the parameter blank to retrieve the current value, or pass a new value as the only parameter to the method. 

For example, to get the status of the on-board LED:

    auto ledState = board.led();

To set the state of the LED:

    board.led(true); // turn on the LED

## Name and Serial Number
Each %Treehopper board has a serial number and a name. Both of these properties can be set by the user. Note that these properties correspond to the ProductName and SerialNumber that are part of the USB specification, which means they'll be visible across your operating system.

## Board Discovery (ConnectionService)
All %Treehopper's C++ API provides a static instance of the ConnectionService class that can be used to retrieve instances to attached %Treehopper boards.

You can use multiple %Treehopper boards simultaneously from a single application. Access these boards from the ConnectionService::instance().boards collection.

## Simultaneous Access
Only one application can connect to a %Treehopper board at a time. This has some important repercussions:
 - <b>Avoid creating instances of Treehopper.ConnectionService; use the static ConnectionService::instance() property it provides for all access</b>. If you want to share a board between different areas of your code, you must share the board object (or the ConnectionService instance that can be used to retrieve the board object). Do not create instances of ConnectionService in each module and attempt to access the boards concurrently; this will fail.
 - <b>When possible, ConnectionService will query the OS --- not the device directly --- about its name and serial number</b>. This allows an application to scan all the boards attached to a computer; even if the boards are in use by other applications. Unfortunately, this is not currently supported in Linux.