#pragma once

namespace Treehopper {

    struct AdcValueChangedEventArgs {
    public:
        int newValue;
    };

    struct AnalogValueChangedEventArgs {
    public:
        double newValue;
    };

    struct AnalogVoltageChangedEventArgs {
    public:
        double newValue;
    };

    class AdcPin
    {

    };
}