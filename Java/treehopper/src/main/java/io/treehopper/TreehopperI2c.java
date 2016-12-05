package io.treehopper;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

/**
 * Created by jay on 12/4/2016.
 */

class TreehopperI2c implements I2c {

    static final Logger logger = LogManager.getLogger("TreehopperUsb");

    boolean enabled;
    double speed = 100.0;
    TreehopperUsb board;

    TreehopperI2c(TreehopperUsb board)
    {
        this.board = board;
    }

    @Override
    public boolean isEnabled() {
        return enabled;
    }

    @Override
    public void setEnabled(boolean enabled) {
        this.enabled = enabled;
        SendConfig();
    }

    @Override
    public double getSpeed() {
        return speed;
    }

    @Override
    public void setSpeed(double speed) {
        this.speed = speed;
        SendConfig();
    }

    private void SendConfig()
    {
        double TH0 = 256.0 - 4000.0 / (3.0 * speed);
        if(TH0 < 0 || TH0 > 255)
        {
            if(TreehopperUsb.Settings.shouldThrowExceptions())
            {
                throw new IllegalArgumentException("Speed out of limits. Valid speeds are 62.5 kHz to 16000 kHz");
            }

            logger.error("Speed out of limits. Valid rate is 62.5 kHz - 16000 kHz (16 MHz)");
        }
        byte[] dataToSend = new byte[3];
        dataToSend[0] = (byte)DeviceCommands.I2cConfig.ordinal();
        dataToSend[1] = (byte)(enabled ? 0x01 : 0x00);
        dataToSend[2] = (byte)Math.round(TH0);
        board.sendPeripheralConfigPacket(dataToSend);
    }

    @Override
    public byte[] sendReceive(byte address, byte[] dataToWrite, int numBytesToRead)
    {
        if(!enabled)
        {
            logger.error("I2c.SendReceive() called before enabling the peripheral. This call will be ignored.");
        }

        if(numBytesToRead > 255)
        {
            logger.error("You may only receive up to 255 bytes per transaction.");
            if(TreehopperUsb.Settings.shouldThrowExceptions())
            {
                throw new IllegalArgumentException("You may only receive up to 255 bytes per transaction.");
            }
        }

        byte[] receivedData = new byte[numBytesToRead];
        int txLen = dataToWrite.length;

        synchronized (board.comsLock)
        {
            byte[] dataToSend = new byte[4 + txLen]; // 2 bytes for the header
            dataToSend[0] = (byte)DeviceCommands.I2cTransaction.ordinal();
            dataToSend[1] = address;
            dataToSend[2] = (byte)txLen; // total length (0-255)
            dataToSend[3] = (byte)numBytesToRead;

            System.arraycopy(dataToWrite, 0, dataToSend, 4, txLen);

            int bytesRemaining = dataToSend.length;
            int offset = 0;

            // for long transactions (> 64 bytes - 4 byte header), we send <=64 byte chunks, one by one.
            while (bytesRemaining > 0)
            {
                int transferLength = bytesRemaining > 64 ? 64 : bytesRemaining;
                byte[] tmp = new byte[transferLength];
                System.arraycopy(dataToSend, offset, tmp, 0, transferLength);
                board.sendPeripheralConfigPacket(tmp);

                offset += transferLength;
                bytesRemaining -= transferLength;
            }

            if (numBytesToRead == 0)
            {
                //var result = device.receiveCommsResponsePacket((uint)1).Result;
                byte[] result = board.receiveCommsResponsePacket(1);
                int resultCode = result[0] & 0xff;
                if (resultCode != 255)
                {
                    I2cTransferError error = I2cTransferError.values()[resultCode];

                    logger.error("I2C transaction resulted in an error: " + error);
                    if(TreehopperUsb.Settings.shouldThrowExceptions())
                        throw new RuntimeException("I2C transaction resulted in an error: " + error);
                }

            } else
            {
                bytesRemaining = numBytesToRead + 1; // received data length + status byte
                int srcIndex = 0;
                byte[] result = new byte[bytesRemaining];
                while (bytesRemaining > 0)
                {
                    int numBytesToTransfer = bytesRemaining > 64 ? 64 : bytesRemaining;

                    byte[] chunk = board.receiveCommsResponsePacket(numBytesToTransfer);
                    System.arraycopy(chunk, 0, result, srcIndex, receivedData.length); // just in case we don't get what we're expecting
                    srcIndex += numBytesToTransfer;
                    bytesRemaining -= numBytesToTransfer;
                }
                int resultCode = result[0] & 0xff;
                if (resultCode != 255)
                {
                    I2cTransferError error = I2cTransferError.values()[resultCode];

                    logger.error("I2C transaction resulted in an error: " + error);
                    if(TreehopperUsb.Settings.shouldThrowExceptions())
                        throw new RuntimeException("I2C transaction resulted in an error: " + error);
                } else
                {
                    System.arraycopy(result, 1, receivedData, 0, numBytesToRead);
                }
            }
        }

        return receivedData;
    }

    enum I2cTransferError
    {
        ArbitrationLostError(0),
        NackError(1),
        UnknownError(2),
        TxunderError(3),
        Success(255);

        private int numVal;

        I2cTransferError(int numVal) {
            this.numVal = numVal;
        }

        public int getNumVal() {
            return numVal;
        }
    }
}
