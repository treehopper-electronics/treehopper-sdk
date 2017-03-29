package io.treehopper.events;

import java.util.HashMap;

import io.treehopper.TreehopperUsb;

/**
 * Created by jay on 12/28/2015.
 */
public interface TreehopperEventsListener {
    void onBoardAdded(TreehopperUsb board);
    void onBoardRemoved(TreehopperUsb board);
}
