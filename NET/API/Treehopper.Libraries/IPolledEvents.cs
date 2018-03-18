using System;
using System.Collections.Generic;
using System.Text;
using Treehopper.Libraries.Sensors;

namespace Treehopper.Libraries
{
    public interface IPolledEvents : IPollable
    {
        int AwaitPollingInterval { get; set; }
    }
}
