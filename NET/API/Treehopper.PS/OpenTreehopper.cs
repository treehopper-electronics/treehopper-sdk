using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.PS
{
    [Cmdlet(VerbsCommon.Open, "Treehopper")]
    public class OpenTreehopper : Cmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public TreehopperUsb[] Board { get; set; }
        protected override void ProcessRecord()
        {
            foreach(var board in Board)
            {
                board.Connect().Wait();
            }
        }
    }
}
