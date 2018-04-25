\page linux Linux
Treehopper's C++ API has Linux support through direct calls into [libusb-1.0](http://libusb.info/), the ubiquitous USB library that's found on almost every Linux device out there.

## Prerequisites
You can build C++ Linux apps that connect to %Treehopper using both the classic [Mono Project](https://www.mono-project.com/) runtime, or the new [.NET Core 2.0](https://www.microsoft.com/net/learn/get-started/linux/rhel) environment.

If you have an old version of .NET Core, please make sure to update it before proceeding; %Treehopper targets .NET Core 2.0 or later.

### udev rules
By default, most Linux-based operating systems restrict normal users from interacting with USB devices. To enable non-root users to interact with %Treehopper boards, you must add a udev rule.

Paste and run this quick snippet into a terminal window:
```
$ sudo echo "SUBSYSTEM==\"usb\", ATTRS{idVendor}==\"10c4\", ATTRS{idProduct}==\"8a7e\", MODE:=\"666\", GROUP=\"users\"" > /etc/udev/rules.d/999-treehopper.rules
```
Verify the rule was created:
```
$ cat /etc/udev/rules.d/999-treehopper.rules 

SUBSYSTEM=="usb", ATTRS{idVendor}=="10c4", ATTRS{idProduct}=="8a7e", MODE:="666", GROUP="users"
```

Note that, for some distributions, `sudo` may not be configured to allow this. You can launch a root shell and repeat the command.
```
$ sudo su
# echo "SUBSYSTEM==\"usb\", ATTRS{idVendor}==\"10c4\", ATTRS{idProduct}==\"8a7e\", MODE:=\"666\", GROUP=\"users\"" > /etc/udev/rules.d/999-treehopper.rules
```

# Blinky in the console
We can build a console application that uses the C++ Treehopper API to blink the on-board LED.

### Step 1: Create a new project
Create a new folder called "blinky" and 