\page linux Linux

# System Requirements
You can build C# Linux apps that connect to %Treehopper using both the classic <a href="https://www.mono-project.com/">Mono Project</a> runtime, or the new <a href="https://www.microsoft.com/net/learn/get-started/linux/rhel">.NET Core</a> environment.

We have tested several software packages and can recommend a 7.x-series version of MonoDevelop as a free option, or IntelliJ Rider IDE if you can justify the expense.

## MonoDevelop Flatpak headaches
For a brief period, MonoDevelop was packaged as a FlatPak image. When MonoDevelop is running in a Flatpak-based environment, debugging an app that uses Treehopper.Desktop will fail, as libusb is unavailable in this execution context.

The MonoDevelop maintainers must have read our minds, because MonoDevelop has returned to standard package repos for most major Linux distributions. If you have an old Flatpak-based MonoDevelop installation, please uninstall it and install a repo-based version, as documented <a href="http://www.monodevelop.com/download/#fndtn-download-lin">here</a>.

# Adding udev rules
By default, most Linux-based operating systems restrict normal users from interacting with USB devices. To enable non-root users to interact with %Treehopper boards, you must a udev rule. 

Paste and run this quick snippet into a terminal window:
\code
> sudo echo "SUBSYSTEM==\"usb\", ATTRS{idVendor}==\"10c4\", ATTRS{idProduct}==\"8a7e\", MODE:=\"666\", GROUP=\"users\"" > /etc/udev/rules.d/999-treehopper.rules
\endcode


Verify the rule was created:
\code
> cat /etc/udev/rules.d/999-treehopper.rules 

SUBSYSTEM=="usb", ATTRS{idVendor}=="10c4", ATTRS{idProduct}=="8a7e", MODE:="666", GROUP="users"
\endcode