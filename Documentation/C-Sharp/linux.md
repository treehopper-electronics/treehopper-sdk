\page linux Linux

# System Requirements
You can build C# Linux apps that connect to %Treehopper using both the classic <a href="https://www.mono-project.com/">Mono Project</a> runtime, or the new <a href="https://www.microsoft.com/net/learn/get-started/linux/rhel">.NET Core</a> environment.

We have tested several software packages and can recommend a 7.x-series version of MonoDevelop as a free option, or IntelliJ Rider IDE if you can justify the expense.

Most Linux distributions have ancient 5.x-series versions of MonoDevelop; these cannot be used to build .NET Standard projects, however, you might be able to create traditional .NET library projects and import code into them (.NET Standard 2.0 is compatible with .NET Framework 4.6.1).

## Flatpak headaches
Unfortunately, MonoDevelop is no longer packaged for individual Linux distributions, but rather, as a FlatPak image. While this works well for web development, the Treehopper.Desktop API can't perform the LibUSB calls necessary to operate the board, which causes debugging to fail (as the app is launched from the context of the FlatPak).