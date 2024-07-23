package io.treehopper.libraries;

public interface IRegisterManagerAdapter {
    byte[] read(int address, int width);

    void write(int address, byte[] data);
}
