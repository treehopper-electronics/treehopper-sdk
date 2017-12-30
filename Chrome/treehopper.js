var vendorId  = 1240;
var productId = 62502;
var interfaceId = 0;
var DEVICE_INFO = {"vendorId": vendorId, "productId": productId, "interfaceId":0};
var TreehopperDevice;

var requestButton = document.getElementById("requestPermission");

var transfer = {
  direction: 'in',
  endpoint: 1 | 128,
  length: 64
};

var onEvent = function(usbEvent) {
    
    if (usbEvent.resultCode) {
      console.log("Error: " + chrome.runtime.lastError.message);
      return;
    }

    var dv = new DataView(usbEvent.data);
    console.log(dv);

  //  chrome.usb.bulkTransfer(TreehopperDevice, transfer, onEvent);
  };


function onDeviceFound(devices) {
  this.devices=devices;
  if (devices) {
    if (devices.length > 0) {
      console.log("Device(s) found: "+devices.length);
    } else {
      console.log("Device could not be found");
    }
  } else {
    console.log("Permission denied.");
  }
}

var permissionObj = {permissions: [{'usbDevices': [DEVICE_INFO] }]};

requestButton.addEventListener('click', function() {
  chrome.permissions.request( permissionObj, function(result) {
    if (result) {
      gotPermission();
    } else {
      console.log('App was not granted the "usbDevices" permission.');
      console.log(chrome.runtime.lastError);
    }
  });
});

var gotPermission = function(result) {
 //   requestButton.style.display = 'none';
 //   knob.style.display = 'block';
    console.log('App was granted the "usbDevices" permission.');
    chrome.usb.findDevices( DEVICE_INFO,
      function(devices) {
        if (!devices || !devices.length) {
          console.log('device not found');
          return;
        }
        console.log('Found device: ' + devices[0].handle);
        TreehopperDevice = devices[0];

        chrome.usb.resetDevice(TreehopperDevice, function()
        {
        chrome.usb.claimInterface(TreehopperDevice, 0, function()
        {
          if(chrome.runtime.lastError)
          {
            console.log(chrome.runtime.lastError.message);
            return;
          }
            chrome.usb.bulkTransfer(TreehopperDevice, transfer, onEvent);
        });  
        })
        
        
    });
  };