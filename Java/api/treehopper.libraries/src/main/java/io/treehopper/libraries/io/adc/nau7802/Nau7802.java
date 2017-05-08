package io.treehopper.libraries.io.adc.nau7802;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.DigitalIn;
import io.treehopper.interfaces.I2c;

/**
 * Created by JayLocal on 5/2/2017.
 */
public class Nau7802 {
    double referenceVoltage;
    SMBusDevice dev;
    Nau7802Registers registers;
    public Nau7802(I2c i2c) {
        dev = new SMBusDevice((byte)0x2A, i2c);
        registers = new Nau7802Registers(dev);

        registers.PuCtrl.RegisterReset = 1;  // reset all registers
        registers.PuCtrl.write();
        registers.PuCtrl.RegisterReset = 0;  // clear reset
        registers.PuCtrl.PowerUpDigital = 1; // power up digital
        registers.PuCtrl.write();

        try {
            Thread.sleep(10);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

        // useful defaults
        registers.PuCtrl.UseExternalCrystal = 0;
        registers.PuCtrl.UseInternalLdo = 1;
        registers.PuCtrl.PowerUpDigital = 1;
        registers.PuCtrl.PowerUpAnalog = 1;
        registers.PuCtrl.write();

        registers.Ctrl1.setVldo(LdoVoltage.mV_3000);
        registers.Ctrl1.setGain(Gains.x1);
        registers.Ctrl1.write();

        updateReferenceVoltage(); // set the pins up with the default gains

        registers.Pga.PgaBypass = 0;
        registers.Pga.DisableChopper = 1;
        registers.Pga.write();

        registers.Adc.setRegChpFreq(RegChpFreqs.off);
        registers.Adc.RegChp = 0;
        registers.Adc.write();

        registers.I2cCtrl.BgpCp = 0;
        registers.I2cCtrl.write();

        registers.Ctrl2.setConversionRate(ConversionRates.Sps_10);
        registers.Ctrl2.write();

        registers.PuCtrl.CycleStart = 1;
        registers.PuCtrl.write();
    }

    public int getAdcValue() {
        return PerformConversion();
    }

    public void setConversionRate(ConversionRates rate) {
        registers.Ctrl2.setConversionRate(rate);
    }

    public void setGain(Gains gain) {
        registers.Ctrl1.setGain(gain);
    }

    private int PerformConversion()
    {
        while(!ConversionDone());

        registers.AdcResult.read();
        return registers.AdcResult.Value;
    }

    private boolean ConversionDone()
    {
        registers.read(registers.PuCtrl);
        return registers.PuCtrl.CycleReady == 1;
    }

    private void updateReferenceVoltage()
    {
        double ldoVoltage = 4.5 - registers.Ctrl1.Vldo * 0.3;
        double gain = 1 << registers.Ctrl1.Gain;

        referenceVoltage = ldoVoltage / gain;
    }

    public boolean calibrate()
    {
        registers.Ctrl2.CalStart = 1;
        registers.Ctrl2.write();
        registers.Ctrl2.CalStart = 1;

        while (registers.Ctrl2.read().CalStart == 1);

        return (registers.Ctrl2.CalError == 0);
    }

    public void SetChannel(int channel)
    {
        registers.Ctrl2.ChannelSelect = channel;
        registers.Ctrl2.write();
        calibrate();
    }
}
