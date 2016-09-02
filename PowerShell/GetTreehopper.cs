using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using Treehopper;
using System.Text.RegularExpressions;

namespace Treehopper.PS
{
    [Cmdlet(VerbsCommon.Get, "Treehopper")]
    public class GetTreehopper : PSCmdlet
    {

        private string[] boardNames;

        [Parameter(
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true
            )]
        [ValidateNotNullOrEmpty]
        public string[] Name
        {
            get { return this.boardNames; }
            set { this.boardNames = value; }
        }


        ConnectionService service = ConnectionService.Instance;
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            service.First().Wait();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            if(boardNames == null)
            {
                WriteObject(service.Boards);
                return;
            }

            foreach(var name in boardNames)
            {
                string pattern = name.ToLower();
                pattern = pattern.Replace(".", "\\.");
                pattern = pattern.Replace("*", ".*?");
                pattern = "^" + pattern + "$";
                Regex regex = new Regex(pattern);
                foreach(var board in service.Boards)
                {
                    if (regex.IsMatch(board.Name.ToLower()))
                        WriteObject(board);
                }
            }
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            service.Dispose();
        }
    }
}
