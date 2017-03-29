package io.treehopper;

/**
 * Created by jay on 12/4/2016.
 */

public class Settings {
    private int vid = 0x10c4;
    private int pid = 0x8a7e;
    private int bootloaderVid = 0x10c4;
    private int bootloaderPid = 0xeac9;
    private boolean throwExceptions = false;

    public int getVid() {
        return vid;
    }

    public void setVid(int vid) {
        this.vid = vid;
    }

    public int getPid() {
        return pid;
    }

    public void setPid(int pid) {
        this.pid = pid;
    }

    public int getBootloaderVid() {
        return bootloaderVid;
    }

    public void setBootloaderVid(int bootloaderVid) {
        this.bootloaderVid = bootloaderVid;
    }

    public int getBootloaderPid() {
        return bootloaderPid;
    }

    public void setBootloaderPid(int bootloaderPid) {
        this.bootloaderPid = bootloaderPid;
    }

    public boolean shouldThrowExceptions() {
        return throwExceptions;
    }

    public void setThrowExceptions(boolean throwExceptions) {
        this.throwExceptions = throwExceptions;
    }
}
