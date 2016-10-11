package io.treehopper.api;

/**
 * Created by jay on 9/30/2016.
 */

public interface PinReportListener {
    void onPinReportReceived(byte[] pinReport);
}
