using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Impworks.BotRunner
{
    /// <summary>
    /// Base class for script runner.
    /// </summary>
    public class Runner<T> where T: StateBase, new()
    {
        public Runner(IEnumerable<Step<T>> steps)
        {
            _steps = steps.ToList();
            if(_steps.Count == 0)
                throw new ArgumentException("At least one step must be defined!", nameof(steps));

            _sessions = new ConcurrentDictionary<int, T>();
        }

        private readonly List<Step<T>> _steps;
        private readonly ConcurrentDictionary<int, T> _sessions;

        /// <summary>
        /// Handles a message according to current step.
        /// </summary>
        public async Task HandleAsync(ITelegramBotClient bot, Message msg)
        {
            var state = _sessions.GetOrAdd(msg.From.Id, uid => new T());
            var step = _steps[state.Step];
            var ctx = new ExecutionContext(bot, msg, state);

            await step.Handler(ctx);

            var nextId = string.IsNullOrEmpty(ctx.NextStepId)
                ? state.Step + 1
                : _steps.FindIndex(x => x.Id == ctx.NextStepId);

            if (nextId < 0 || nextId >= _steps.Count)
                _sessions.TryRemove(msg.From.Id, out _);

            state.Step = nextId;
        }

        /// <summary>
        /// Context for handling a message.
        /// </summary>
        private class ExecutionContext : IExecutionContext<T>
        {
            public ExecutionContext(ITelegramBotClient bot, Message msg, T state)
            {
                Bot = bot;
                Message = msg;
                State = state;
            }

            public string NextStepId { get; set; }

            public Message Message { get; }
            public T State { get; }
            public ITelegramBotClient Bot { get;  }

            /// <summary>
            /// Sets the next step explicitly.
            /// </summary>
            public void Goto(string stepId)
            {
                NextStepId = stepId;
            }
        }
    }
}
