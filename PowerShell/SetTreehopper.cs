using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.PS
{
    [Cmdlet(VerbsCommon.Set, "Treehopper")]
    public class SetTreehopper : PSCmdlet
    {
        [Parameter(ParameterSetName = "Led")]
        public bool Led { get; set; }

        [Parameter(ParameterSetName = "Mode")]
        public string Mode { get; set; }

        [Parameter(ParameterSetName = "Value")]
        public bool Value { get; set; }

        [Parameter(ParameterSetName = "Mode")]
        [Parameter(ParameterSetName = "Value")]
        public int[] Pin { get; set; }

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public TreehopperUsb[] Board { get; set; }
        protected override void ProcessRecord()
        {
            foreach(var board in Board)
            {

                board.ConnectAsync().Wait();
                switch(ParameterSetName)
                {
                    case "Led":
                        board.Led = Led;
                        break;

                    case "Value":
                        foreach (int pin in Pin)
                        {
                            board.Pins[pin].DigitalValue = Value;
                        }
                        break;

                    case "Mode":
                        foreach (int pin in Pin)
                        {
                            switch(Mode.ToLower())
                            {
                                case "analoginput":
                                case "analogin":
                                case "analog":
                                    board.Pins[pin].Mode = PinMode.AnalogInput;
                                    break;

                                case "digitalinput":
                                case "digitalin":
                                case "in":
                                    board.Pins[pin].Mode = PinMode.DigitalInput;
                                    break;

                                case "digitalout":
                                case "digitaloutput":
                                case "pushpulloutput":
                                case "digitalpushpull":
                                case "out":
                                    board.Pins[pin].Mode = PinMode.PushPullOutput;
                                    break;

                                case "opendrain":
                                case "digitalopendrain":
                                case "opendrainoutput":
                                    board.Pins[pin].Mode = PinMode.OpenDrainOutput;
                                    break;
                            }
                            
                        }
                        break;
                }
            }
        }
    }
}
