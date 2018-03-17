\page linux Linux

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