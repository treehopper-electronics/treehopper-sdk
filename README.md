# Treehopper
Treehopper connects the physical world to your computer, smartphone, or tablet. Treehopper lets your smartphone, tablet, or computer work like a microcontroller — complete with up to 20 pins of digial and analog I/O, PWM, SPI, I2C, and UART.

Find out more at [treehopper.io](https://treehopper.io)

## About This Repo
This repo contains both the firmware source code that runs on the Treehopper board, as well as source code for the SDKs for C++, Java, .NET, and Python --- plus some examples for other environments (like MATLAB).

## Issue Tracking
We use the GitHub issue tracker for tracking bugs and feature requests for the SDKs and board firmware. **The issue tracker is not a support forum for users**; instead, Treehopper users looking for general support should use our [Treehopper Community Forums](https://community.treehopper.io) or our [Gitter help room](https://gitter.im/treehopper-electronics/help).

## Library Support
We love the idea of a centralized library that supports hundreds of peripherals, but maintaining this across all platforms is a huge undertaking --- please be patient when reporting bugs in library code.

If there's a new chip out in the wild that you want to use, please consider contributing a library PR for it. We prefer a full C#/C++/Java/Python implementation, but even a single-language contribution goes a long way. 

If you're not comfortable writing your own peripheral driver, you may file a library-request issue. Please understand that we consider new library requests to be low priority. If the library is for an expensive or unusual peripheral, we may request you send us loaner hardware — otherwise, the library may *never* get implemented.