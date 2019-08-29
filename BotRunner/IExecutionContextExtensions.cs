using System.Threading.Tasks;

namespace Impworks.BotRunner
{
    public static class ExecutionContextExtensions
    {
        /// <summary>
        /// Sends a text reply to the current message.
        /// </summary>
        public static async Task Reply<T>(this IExecutionContext<T> ctx, string text)
            where T: StateBase
        {
            await ctx.Bot.SendTextMessageAsync(ctx.Message.Chat.Id, text);
        }
    }
}
