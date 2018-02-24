using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Treehopper;

namespace TreehopperControlCenter
{
    public class PinViewModel : INotifyPropertyChanged
    {
        private Pin pin;

        public List<string> PinModes { get; set; } = new List<String> { "Digital Input", "Analog Input", "Digital Output", "SoftPWM" };

        private string selectedPinMode = "";

        public event PropertyChangedEventHandler PropertyChanged;

        public string SelectedPinMode { 
            get
            {
                return selectedPinMode;
            }

            set
            {
                selectedPinMode = value;
                switch(selectedPinMode)
                {
                    case "Digital Input":
                        if (pin != null)
                        {
                            pin.Mode = PinMode.DigitalInput;
                        }
                        SwitchVisible = false;
                        DigitalInputVisible = true;
                        SliderVisible = false;
                        ProgressVisible = false;
                        SwitchEnabled = false;
                        break;

                    case "Analog Input":
                        if (pin != null)
                        {
                            pin.Mode = PinMode.AnalogInput;
                        }
                        SwitchVisible = false;
                        DigitalInputVisible = false;
                        SliderVisible = false;
                        ProgressVisible = true;
                        SwitchEnabled = false;
                        break;

                    case "Digital Output":

                        if (pin != null)
                        {
                            pin.Mode = PinMode.PushPullOutput; 
                        }
                        SwitchVisible = true;
                        DigitalInputVisible = false;
                        SliderVisible = false;
                        ProgressVisible = false;
                        SwitchEnabled = true;
                        break;

                    case "SoftPWM":
                        if (pin != null) pin.Mode = PinMode.SoftPwm;
                        SwitchVisible = false;
                        DigitalInputVisible = false;
                        SliderVisible = true;
                        ProgressVisible = false;
                        SwitchEnabled = false;
                        break;
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SwitchVisible"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SliderVisible"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProgressVisible"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SwitchEnabled"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DigitalInputVisible"));
            }
        }

        public string Name { get; set; }

        private double sliderValue;

        public double SliderValue
        {
            get { return sliderValue; }
            set {
                sliderValue = value;
                if (pin.Mode == PinMode.SoftPwm)
                    pin.DutyCycle = sliderValue;
            }
        }

        private bool switchValue;

        public bool SwitchValue
        {
            get { return switchValue; }
            set {
                switchValue = value;
                if(pin.Mode == PinMode.PushPullOutput)
                    pin.DigitalValue = switchValue;
            }
        }

        private double progressValue;

        public double ProgressValue
        {
            get { return progressValue; }
            set { progressValue = value; }
        }

        public PinViewModel()
        {
            Name = "test pin";
            SelectedPinMode = PinModes[0];
        }

        public bool DigitalInputValue { get; set; }


        public PinViewModel(Pin pin)
        {
            this.pin = pin;
            Name = pin.Name;
            pin.DigitalValueChanged += Pin_DigitalValueChanged;
            pin.AnalogValueChanged += Pin_AnalogValueChanged;

            SelectedPinMode = PinModes[0];
        }

        private void Pin_AnalogValueChanged(object sender, AnalogValueChangedEventArgs e)
        {
            ProgressValue = e.NewValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProgressValue"));
        }

        private void Pin_DigitalValueChanged(object sender, DigitalInValueChangedEventArgs e)
        {
            DigitalInputValue = e.NewValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DigitalInputValue"));
        }

        public bool SwitchEnabled { get; set; }
        public bool SwitchVisible { get; set; }
        public bool SliderVisible { get; set; }
        public bool ProgressVisible { get; set; }
        public bool DigitalInputVisible { get; set; }

    }
}
