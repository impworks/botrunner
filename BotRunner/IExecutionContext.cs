using Telegram.Bot;
using Telegram.Bot.Types;

namespace Impworks.BotRunner
{
    /// <summary>
    /// Control points for the step handler.
    /// </summary>
    public interface IExecutionContext<out T> where T: StateBase
    {
        /// <summary>
        /// Executes the step next.
        /// </summary>
        void Goto(string stepId);

        /// <summary>
        /// Bot client instance.
        /// </summary>
        ITelegramBotClient Bot { get; }

        /// <summary>
        /// User session.
        /// </summary>
        T State { get; }

        /// <summary>
        /// Current message from the user.
        /// </summary>
        Message Message { get; }
    }
}
