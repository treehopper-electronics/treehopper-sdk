package io.treehopper;

/**
 * Settings used by Treehopper classes
 */
public class Settings {
    private int vid = 0x10c4;
    private int pid = 0x8a7e;
    private int bootloaderVid = 0x10c4;
    private int bootloaderPid = 0xeac9;
    private boolean throwExceptions = false;

    /**
     * Get the VID to look for
     * @return the vid
     */
    public int getVid() {
        return vid;
    }

    /**
     * Set the VID to look for
     * @param vid the vid to search for
     */
    public void setVid(int vid) {
        this.vid = vid;
    }

    /**
     * Get the PID to look for
     * @return the PID
     */
    public int getPid() {
        return pid;
    }

    /**
     * Set the PID to look for
     * @param pid the PID
     */
    public void setPid(int pid) {
        this.pid = pid;
    }

    /**
     * Get the bootloader VID
     * @return the VID of the bootloader
     */
    public int getBootloaderVid() {
        return bootloaderVid;
    }

    /**
     * Set the bootloader VID
     * @param bootloaderVid the VID of the bootloader
     */
    public void setBootloaderVid(int bootloaderVid) {
        this.bootloaderVid = bootloaderVid;
    }

    /**
     * Get the bootloader PID
     * @return the bootloader PID
     */
    public int getBootloaderPid() {
        return bootloaderPid;
    }

    /**
     * Set the bootloader PID
     * @param bootloaderPid the bootloader PID
     */
    public void setBootloaderPid(int bootloaderPid) {
        this.bootloaderPid = bootloaderPid;
    }

    /**
     * Gets whether Treehopper should throw exceptions or silently fail
     * @return whether to throw exceptions
     */
    public boolean shouldThrowExceptions() {
        return throwExceptions;
    }

    /**
     * Sets whether Treehopper should throw exceptions or silently fail
     * @param throwExceptions whether to throw exceptions
     */
    public void setThrowExceptions(boolean throwExceptions) {
        this.throwExceptions = throwExceptions;
    }
}
