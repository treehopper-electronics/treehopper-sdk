using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.PS
{
    [Cmdlet(VerbsCommon.Close, "Treehopper")]
    public class CloseTreehopper : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public TreehopperUsb[] Board { get; set; }
        protected override void ProcessRecord()
        {
            foreach (var board in Board)
            {
                board.Disconnect();
            }
        }
    }
}
