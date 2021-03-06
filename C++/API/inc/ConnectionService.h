#pragma once

#include "Treehopper.h"
#include "TreehopperUsb.h"
#include "UsbConnection.h"

#ifdef __APPLE__
#include <IOKit/IOKitLib.h>
#include <IOKit/IOTypes.h>
#include <CoreFoundation/CFRunLoop.h>
#endif
#ifdef __linux__
#include "libusb-1.0/libusb.h"
#endif

#include <vector>

namespace Treehopper {
    /** Provides TreehopperUsb discovery and factory duties. */
    class TREEHOPPER_API ConnectionService {
    public:
        ConnectionService();

        ~ConnectionService();

        /** Gets the ConnectionService instance to use for TreehopperUsb discovery. */
        static ConnectionService &instance() {
            static ConnectionService instance;
            return instance;
        }

        /** Gets the first TreehopperUsb board connected to the computer */
        TreehopperUsb& getFirstDevice();

        /** Gets the collection of TreehopperUsb boards connected to the computer */
        vector<TreehopperUsb> boards;
    private:
        void scan();

#ifdef __linux__
        libusb_context *context;
#endif

#ifdef __APPLE__
        CFRunLoopRef gRunLoop;
        static void DeviceAdded(void *refCon, io_iterator_t iterator);
        static void DeviceRemoved(void *refCon, io_service_t service, natural_t messageType, void *messageArgument);
        static std::mutex boardCollectionMutex;
        static std::condition_variable boardCollectionCondition;
        static std::thread deviceListenerThread;
        static IONotificationPortRef gNotifyPort;
#endif
    };
}

