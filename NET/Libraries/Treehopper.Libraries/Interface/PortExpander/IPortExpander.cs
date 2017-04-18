using System.Threading.Tasks;
using Treehopper.Libraries.Sensors;

namespace Treehopper.Libraries.Interface.PortExpander
{
    /// <summary>
    /// A port expander
    /// </summary>
    /// <typeparam name="TPortExpanderPin">The port expander's pin type</typeparam>
    public interface IPortExpander<TPortExpanderPin> : IFlushableOutputPort<TPortExpanderPin> where TPortExpanderPin : IPortExpanderPin
    {
    }

    /// <summary>
    /// Parent port expander type. Separated from IPortExpander to eliminate circular references
    /// </summary>
    public interface IPortExpanderParent : IPollable
    {
        /// <summary>
        /// Called by a port expander pin when its value needs to be updated
        /// </summary>
        /// <param name="portExpanderPin">The port expander pin to update</param>
        Task OutputValueChanged(IPortExpanderPin portExpanderPin);

        /// <summary>
        /// Called by a port expander pin when its value needs to be updated
        /// </summary>
        /// <param name="portExpanderPin"></param>
        Task OutputModeChanged(IPortExpanderPin portExpanderPin);
    }
}
