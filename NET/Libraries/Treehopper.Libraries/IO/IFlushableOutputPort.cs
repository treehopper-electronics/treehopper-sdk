using System.Collections.Generic;

namespace Treehopper.Libraries.Interface
{
    /// <summary>
    ///     Represents a flushable output port
    /// </summary>
    /// <typeparam name="TDigitalPin"></typeparam>
    public interface IFlushableOutputPort<TDigitalPin> : IFlushable where TDigitalPin : DigitalOut
    {
        /// <summary>
        ///     Collection of pins associated with this output port
        /// </summary>
        IList<TDigitalPin> Pins { get; }
    }
}