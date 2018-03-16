\page linux Linux

# System Requirements
Treehopper's C# core API and libraries API are built in .NET Standard 2.0, and we are transitioning Treehopper.Desktop to this system, too. This will allow Treehopper apps to be built using the cross-platform, lightweight, open-source .NET Core tooling.

We have tested several software packages, and can recommend a 7.x-series version of MonoDevelop as a free option, or IntelliJ Rider IDE if you can justify the expense.

Most Linux distributions have ancient 5.x-series versions of MonoDevelop; these cannot be used to build .NET Standard projects, however, you might be able to create traditional .NET library projects and import code into them (.NET Standard 2.0 is compatible with .NET Framework 4.6.1).

## Flatpak headaches
Unfortunately, MonoDevelop is no longer packaged for individual Linux distributions, but rather, as a FlatPak image. While this works well for web development, the Treehopper.Desktop API can't perform the LibUSB calls necessary to operate the board, which causes debugging to fail (as the app is launched from the context of the FlatPak).