package io.treehopper.events;

import io.treehopper.TreehopperUsb;

/**
 * Handler for TreehopperEvents
 */
public interface TreehopperEventsHandler {
    void onBoardAdded(TreehopperUsb board);

    void onBoardRemoved(TreehopperUsb board);
}
