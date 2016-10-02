package io.treehopper.api;

import java.util.HashMap;

/**
 * Created by jay on 12/28/2015.
 */
public interface TreehopperEventsListener {
    void onBoardAdded(TreehopperUsb board);
    void onBoardRemoved(TreehopperUsb board);
}
