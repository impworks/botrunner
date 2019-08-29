using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Impworks.BotRunner
{
    /// <summary>
    /// Helper class for dynamically building runner steps.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RunnerBuilder<T> where T: StateBase, new()
    {
        private readonly List<Step<T>> _steps = new List<Step<T>>();

        /// <summary>
        /// Adds a new step without a name.
        /// </summary>
        public RunnerBuilder<T> Step(Func<IExecutionContext<T>, Task> handler)
        {
            return Step(Guid.NewGuid().ToString(), handler);
        }

        /// <summary>
        /// Adds a new step with a name.
        /// </summary>
        public RunnerBuilder<T> Step(string id, Func<IExecutionContext<T>, Task> handler)
        {
            _steps.Add(new Step<T>(id, handler));
            return this;
        }

        /// <summary>
        /// Creates the runner with defined steps.
        /// </summary>
        public Runner<T> Build()
        {
            return new Runner<T>(_steps);
        }
    }
}
