\page windows Windows

# Prerequisites
Treehopper's C++ API is not distributed in binary form; you will need to build it before including and linking to your project.

# Using vcpkg
You can use Microsoft's open-source vcpkg tool to install more than 900 different commonly-used C++ libraries into your project.

## Step 1
Clone or download the (https://github.com/Microsoft/vcpkg)[vcpkg repo]. Here, we'll assume you cloned to `C:\src\vcpkg`.

## Step 2
Build the `vcpkg` executable

```
C:\src\vcpkg> .\bootstrap-vcpkg.bat
```

## Step 3
Integrate vcpkg into Visual Studio:

```
C:\src\vcpkg> .\vcpkg integrate install
```

## Step 4
Install the Treehopper package:

```
C:\src\vcpkg> .\vcpkg install treehopper
```

Treehopper has no additional run-time requirements, as it calls directly into native Win32 functions for all USB communication.

