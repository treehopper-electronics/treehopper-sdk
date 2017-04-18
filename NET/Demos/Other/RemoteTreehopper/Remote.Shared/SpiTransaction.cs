using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Newtonsoft.Json;
namespace Remote.Shared
{
    public class SpiTransaction
    {
        public SpiTransaction()
        {

        }

        public SpiTransaction(string json)
        {
            var temp = JsonConvert.DeserializeObject<SpiTransaction>(json);
            Burst = temp.Burst;
            ChipSelectPinNumber = temp.ChipSelectPinNumber;
            ChipSelectMode = temp.ChipSelectMode;
            DataToWrite = temp.DataToWrite;
            Speed = temp.Speed;
            SpiMode = temp.SpiMode;
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        [JsonProperty("d")]
        public byte[] DataToWrite { get; set; }

        [JsonProperty("csp")]
        public int ChipSelectPinNumber { get; set; }

        [JsonProperty("csm")]
        public ChipSelectMode ChipSelectMode { get; set; }

        [JsonProperty("s")]
        public double Speed { get; set; }

        [JsonProperty("b")]
        public SpiBurstMode Burst { get; set; }

        [JsonProperty("m")]
        public SpiMode SpiMode { get; set; }
    }
}
