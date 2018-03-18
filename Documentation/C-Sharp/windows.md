\page Windows

# Introduction
Treehopper provides first-class support for building console and desktop Windows apps that target either class Win32 APIs, or the Windows 10 Universal Windows Platform (UWP). 

# Getting Started with Win32
If you're building a 

# Getting Started with UWP

## Adding a DeviceCapability
The normal appxmanifest GUI used for configuring capabilities cannot be used to configure USB DeviceCapabilities. Instead, right-click on the Package.appxmanifest file and choose View Code.

Find or add the `Capabilities` section and add
```
<DeviceCapability Name="usb">
    <Device Id="vidpid:10c4 8a7e">
        <Function Type="classId:ff * *" />
    </Device>
</DeviceCapability>
```
