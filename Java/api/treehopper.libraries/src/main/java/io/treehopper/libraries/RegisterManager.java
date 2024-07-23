package io.treehopper.libraries;

import java.util.ArrayList;
import java.util.Arrays;

public class RegisterManager {
    protected ArrayList<Register> _registers = new ArrayList<>();
    protected IRegisterManagerAdapter _adapter;

    public RegisterManager(IRegisterManagerAdapter adapter) {
        _adapter = adapter;
    }

    public void read(Register register) {
        register.setBytes(_adapter.read(register.address, register.width));
    }

    public void readRange(Register start, Register end) {
        int count = (end.address + end.width) - start.address;
        byte[] bytes = _adapter.read(start.address, count);
        int i = 0;

        for (Register reg : _registers) {
            if (reg.address < start.address || reg.address > end.address)
                continue;

            reg.setBytes(Arrays.copyOfRange(bytes, i, reg.width + i));
            i += reg.width;
        }
    }

    public void write(Register register) {
        _adapter.write(register.address, register.getBytes());
    }

    public void writeRange(Register start, Register end) {
        ArrayList<Byte> bytes = new ArrayList<>();
        for (Register reg : _registers) {
            if (reg.address < start.address || reg.address > end.address)
                continue;

            byte[] readBytes = reg.getBytes();
            for (int i = 0; i < readBytes.length; i++)
                bytes.add(readBytes[i]);
        }

        byte[] bytesToWrite = new byte[bytes.size()];
        for (int i = 0; i < bytes.size(); i++) {
            bytesToWrite[i] = bytes.get(i);
        }

        _adapter.write(start.address, bytesToWrite);
    }
}
