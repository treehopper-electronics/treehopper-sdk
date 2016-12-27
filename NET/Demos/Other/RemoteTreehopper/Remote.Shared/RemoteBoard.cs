using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Remote.Shared
{
    public class RemoteBoardInfo
    {
        public RemoteBoardInfo()
        {

        }
        public RemoteBoardInfo(TreehopperUsb board)
        {
            this.Name = board.Name;
            this.Serial = board.SerialNumber;
        }

        public RemoteBoardInfo(string json)
        {
            var board = JsonConvert.DeserializeObject<RemoteBoardInfo>(json);
            this.Name = board.Name;
            this.Serial = board.Serial;
        }
        public string Name { get; set; }
        public string Serial { get; set; }

        public override string ToString()
        {
            return Name + " (" + Serial + ")";
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
