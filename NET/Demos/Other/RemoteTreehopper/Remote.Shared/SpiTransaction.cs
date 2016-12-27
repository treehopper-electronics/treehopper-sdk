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
            this.Burst = temp.Burst;
            this.ChipSelectPinNumber = temp.ChipSelectPinNumber;
            this.ChipSelectMode = temp.ChipSelectMode;
            this.DataToWrite = temp.DataToWrite;
            this.Speed = temp.Speed;
            this.SpiMode = temp.SpiMode;
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
        public BurstMode Burst { get; set; }

        [JsonProperty("m")]
        public SpiMode SpiMode { get; set; }
    }
}
