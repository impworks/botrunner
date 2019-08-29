using System;
using System.Threading.Tasks;

namespace Impworks.BotRunner
{
    /// <summary>
    /// Definition of a dialog step.
    /// </summary>
    public class Step<T> where T: StateBase
    {
        public Step(string id, Func<IExecutionContext<T>, Task> handler)
        {
            Id = id;
            Handler = handler;
        }

        /// <summary>
        /// Unique ID of the step.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Handler for the message.
        /// </summary>
        public Func<IExecutionContext<T>, Task> Handler { get; }
    }
}
