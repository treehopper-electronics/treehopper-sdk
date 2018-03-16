\page libraries Libraries

## UpdateAsync() and AutoUpdateWhenPropertyRead
Many sensors require several milliseconds or longer to update their values; to make the API both easy to use and also performant, Treehopper provides control over how sensor properties are updated.

By default, reading from a sensor's property (say, the _Accelerometer_ property of an IMU) will fetch an update from the sensor and set the property's value to this reading. 

This is convenient for console scripts and simple applications, but for more efficient bus usage, consider changing the sensor's AutoUpdateWhenPropertyRead property to false, which allows you to read from properties without fetching updates.

When you *do* want to request an update, call the sensor's UpdateAsync() method. This method will update all the properties, and fire PropertyChanged events where appropriate.

## Directly binding to sensor properties
Because most library components expose PropertyChanged-capable properties that hold the object's state, you can directly bind UI controls --- like text boxes, list views, buttons, progress bars, and sliders --- to library component properties.

## AutoFlush
Similarly, for *output* devices, changing a device's property will write out this value to the device immediately by default. Many devices --- like LED arrays, shift registers, and graphical LCDs --- require the entire peripheral to be updated at once.

To enable more control over how peripheral updates are flushed out, you can disable this functionality by setting AutoFlush to false. Then, whenever you wish to flush writes out to the device, call the FlushAsync() method.