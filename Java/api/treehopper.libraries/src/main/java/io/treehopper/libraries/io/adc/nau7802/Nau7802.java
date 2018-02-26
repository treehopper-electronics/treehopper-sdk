package io.treehopper.libraries.io.adc.nau7802;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;

/**
 * Created by JayLocal on 5/2/2017.
 */
public class Nau7802 {
    double referenceVoltage;
    SMBusDevice dev;
    Nau7802Registers registers;

    public Nau7802(I2c i2c) {
        dev = new SMBusDevice((byte) 0x2A, i2c);
        registers = new Nau7802Registers(dev);

        registers.puCtrl.registerReset = 1;  // reset all registers
        registers.puCtrl.write();
        registers.puCtrl.registerReset = 0;  // clear reset
        registers.puCtrl.powerUpDigital = 1; // power up digital
        registers.puCtrl.write();

        try {
            Thread.sleep(10);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

        // useful defaults
        registers.puCtrl.useExternalCrystal = 0;
        registers.puCtrl.useInternalLdo = 1;
        registers.puCtrl.powerUpDigital = 1;
        registers.puCtrl.powerUpAnalog = 1;
        registers.puCtrl.write();

        registers.ctrl1.setVldo(Vldoes.mV_3000);
        registers.ctrl1.setGain(Gains.x1);
        registers.ctrl1.write();

        updateReferenceVoltage(); // set the pins up with the default gains

        registers.pga.pgaBypass = 0;
        registers.pga.disableChopper = 1;
        registers.pga.write();

        registers.adc.setRegChpFreq(RegChpFreqs.off);
        registers.adc.regChp = 0;
        registers.adc.write();

        registers.i2cCtrl.bgpCp = 0;
        registers.i2cCtrl.write();

        registers.ctrl2.setConversionRate(ConversionRates.Sps_10);
        registers.ctrl2.write();

        registers.puCtrl.cycleStart = 1;
        registers.puCtrl.write();
    }

    public int getAdcValue() {
        return PerformConversion();
    }

    public void setConversionRate(ConversionRates rate) {
        registers.ctrl2.setConversionRate(rate);
    }

    public void setGain(Gains gain) {
        registers.ctrl1.setGain(gain);
    }

    private int PerformConversion() {
        while (!ConversionDone()) ;

        registers.adcResult.read();
        return registers.adcResult.value;
    }

    private boolean ConversionDone() {
        registers.read(registers.puCtrl);
        return registers.puCtrl.cycleReady == 1;
    }

    private void updateReferenceVoltage() {
        double ldoVoltage = 4.5 - registers.ctrl1.vldo * 0.3;
        double gain = 1 << registers.ctrl1.gain;

        referenceVoltage = ldoVoltage / gain;
    }

    public boolean calibrate() {
        registers.ctrl2.calStart = 1;
        registers.ctrl2.write();
        registers.ctrl2.calStart = 1;

        while (registers.ctrl2.read().calStart == 1) ;

        return (registers.ctrl2.calError == 0);
    }

    public void SetChannel(int channel) {
        registers.ctrl2.channelSelect = channel;
        registers.ctrl2.write();
        calibrate();
    }
}
