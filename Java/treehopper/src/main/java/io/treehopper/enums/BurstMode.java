package io.treehopper.enums;

/**
 * Created by jay on 12/6/2016.
 */

/// <summary>
/// The SPI burst mode to use
/// </summary>
public enum BurstMode
{
    /// <summary>
    /// No burst -- always read the same number of bytes as transmitted
    /// </summary>
    NoBurst,

    /// <summary>
    /// Transmit burst -- don't return any data read from the bus
    /// </summary>
    BurstTx,

    /// <summary>
    /// Receive burst -- ignore transmitted data above 53 bytes long, but receive the full number of bytes specified
    /// </summary>
    BurstRx
}
