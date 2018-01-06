\page Treehopper App

\info If you want to build and debug the Android version of the app, we've found you must call `adb shell setprop debug.mono.runtime_args "-O=-simd"` in an ADB prompt to disable SIMD when operating in VMWare. Otherwise, when the Activator attempts to create instances of the library components in the LibrariesPage constructor, the app will tank.