\page hosts Supported Hosts

This page goes over host requirements, and provides a few examples of supported devices. We provide some platform-specific instructions on our \ref getting-started pages.

# Desktop and Laptop Computers {#desktop}
Treehopper supports essentially all desktops and laptops with USB ports. The only software requirements are that the device runs:
 - **Windows 7 or newer:** This is needed for automatic WinUSB installation, but an adventurous user may be able to get older Windows systems working. Windows 10 is recommended, since it provides support for UWP app development. Windows 8 or newer is also needed for named WinUSB-based devices (so your device will appear in your operating system as "Treehopper" or whatever custom name you choose). In Windows 7, the device will always appear as "WinUsb Device."
 - **macOS 10.4 or newer:** The C# and C++ API directly call into macOS's native IOKit-based USB access layer, so there's no additional software needed on macOS. If you're using Python or Java APIs, you'll need to install LibUSB.
 - **Linux:** Treehopper language APIs support Linux through LibUSB. Unless you are targeting an extremely resource-constrained device with a custom root filesystem, you'll most certainly have LibUSB already installed. We regularly test on Linux, but LibUSB also supports OpenBSD, NetBSD, and FreeBSD, so our libraries should work there, too --- perhaps with minor patching for dynamic runtime OS detection (needed in the case of the C# API).

# Windows tablets, phones, and set-top-boxes {#windows-tablets-phones}
Most Windows tablets work great for running Treehopper projects, as nearly all of them have USB OTG support. All current-generation Windows 10 tablets on the market use Intel Atom processors and run the full x86-based version of Windows. This means they can be used with all the Treehopper language APIs seamlessly -- C# Desktop, Windows UWP, Python, Java, or C++. And since these tablets are the same architecture as your desktop or laptop computer, you can literally drag-and-drop compiled apps -- even ones written in native C++ -- from your computer to the tablet to deploy.

An emerging category of device is the Windows 10 set-top-box and the Windows 10 compute stick form-factors. We regularly test with various offerings from [MeeGoPad](http://www.x86pad.com/), [Wintel](https://www.amazon.com/dp/B06W2LWQKC), and [Intel](http://www.intel.com/content/www/us/en/compute-stick/intel-compute-stick.html). Many of these devices are in the $100 price range, and while there are issues with using these devices for general computing (many have very limited RAM), these can work well for dedicated deployed projects.

For Windows phones, the Lumia 950, 950 XL, and HP Elite X3 have USB-C connectors, which simplifies cabling substantially. Other phones with USB OTG support include the Alcatel Idol 4S and the CUBE WP10, though these will need a microUSB-to-A host adapter, and a separate USB-A-to-C cable. Most USB C-to-micro cables are wired for device, and not host, roles on the USB micro side, so they will not work.

# Android Tablets, Smartphones, and Set-top-boxes {#android}
The most convenient Android experience will come from a high-end phone with a USB C connector on it. These allow direct connections to Treehopper using a USB C cable.

Having said that, any Android device running Lollipop (5.0 -- API Level 21) with USB Host/USB OTG capability can run Treehopper. All high-end devices have this functionality, and many new lower-end and middle-tier devices can also serve as USB hosts.

Unfortunately, this feature is often not prominently advertised, and it can vary by carrier or hardware revision --- even on devices that are otherwise identical. If you want to check the devices you already own, there are several apps on the Google Play Store that do this. We've tried both [OTG?](https://play.google.com/store/apps/details?id=com.btssm.doihaveotg&hl=en) and [USB Host Diagnostics](https://play.google.com/store/apps/details?id=eu.chainfire.usbhostdiagnostics&hl=en).

If you're looking for a low-cost Android phone you can leave in your project, we've found the [Moto E (2nd Generation)](https://www.gsmarena.com/motorola_moto_e_(2nd_gen)-6986.php) is one of the best values. These phones are sold as both carrier-locked and unlocked devices, and are also popular with prepaid plans. We've seen them new in the ~$20 range (Tracfone), but unlocked versions go for $65 new or so. On the used market, they're often less than $30.

## Generic Android Set-Top Boxes
If you don't need portability, there are tons of sub-$30 Android set-top boxes on popular sites like Amazon and eBay. These boxes can be *very* slow for their stated purposes (Netflix, gaming) but quite usable for simple apps. Many of them have awkward launchers designed for display on a television (and a remote control as the sole input).

# Embedded Devices {#embedded-linux}
Many embedded devices running Linux or Windows can be used with Treehopper. The [Raspberry Pi](https://www.raspberrypi.org/) is one such device, and, when running Linux, it works well as a host for apps that use Treehopper -- whether written in C# (via .NET Core or Mono), Python, Java, or C++.

We also support Raspberry Pi running [Windows 10 IoT Core](https://developer.microsoft.com/en-us/windows/iot/explore/iotcore) through our C# UWP API. This framework has nice out-of-the-box support for deployment and remote management --- something the Pi, a general-purpose computer, doesn't have with its default Linux distributions.

Even low-cost resource-constrained devices --- like the $20 OpenWRT-powered MIPS routers out there --- can run Treehopper apps written in C++.

# iOS Support? {#ios}
Unfortunately, Treehopper cannot be used with Apple iPhones or iPads. This is less of a technical limitation with iOS, but more a legal/licensure issue that relates to the MFi (Made for iOS) program. Apple does not allow arbitrary iOS apps to connect to hardware over the Lightning connector -- so while Treehopper Electronics could theoretically certify Treehopper as an MFi device, only Treehopper Electronics could publish iOS apps that connect to the device, which wouldn't be useful for the Treehopper developer community.