package io.treehopper;

import io.treehopper.enums.BurstMode;
import io.treehopper.enums.ChipSelectMode;
import io.treehopper.enums.PinMode;
import io.treehopper.enums.SpiMode;

/**
 * Created by jay on 12/7/2016.
 */

public class SpiDevice {
    private final Pin chipSelect;
    private final Spi spi;
    private final double frequency;
    private final SpiMode mode;
    private final ChipSelectMode chipSelectMode;

    public SpiDevice(Spi spiModule, Pin chipSelect, ChipSelectMode csMode)
    {
        this(spiModule, chipSelect, csMode, 1, SpiMode.Mode00);
    }

    public SpiDevice(Spi spiModule, Pin chipSelect, ChipSelectMode csMode, double speedMHz)
    {
        this(spiModule, chipSelect, csMode, speedMHz, SpiMode.Mode00);
    }

    public SpiDevice(Spi spiModule, Pin chipSelect, ChipSelectMode csMode, double speedMHz, SpiMode mode)
    {
        this.chipSelectMode = csMode;
        this.chipSelect = chipSelect;
        this.spi = spiModule;
        this.frequency = speedMHz;
        this.mode = mode;

        this.chipSelect.setMode(PinMode.PushPullOutput);

        spi.setEnabled(true);
    }

    public byte[] SendReceive(byte[] dataToSend)
    {
        byte[] retVal = new byte[dataToSend.length];

        // We need to lock this in case another thread tries to step in and do a transaction with different settings
        return spi.SendReceive(dataToSend, chipSelect, chipSelectMode, frequency, BurstMode.NoBurst, mode);
    }

    public byte[] SendReceive(byte[] dataToSend, BurstMode burst)
    {
        byte[] retVal = new byte[dataToSend.length];

        // We need to lock this in case another thread tries to step in and do a transaction with different settings
        return spi.SendReceive(dataToSend, chipSelect, chipSelectMode, frequency, burst, mode);
    }
}
