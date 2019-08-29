# BotRunner
Tiny framework for programming Telegram Bots

### What is it?
This is a framework for scripting bot dialogs in Telegram based on <a href="https://github.com/TelegramBots/telegram.bot">Telegram.Bot library</a>.
It is based on a state machine: the dialog consists of steps, which can be executed sequentially or in a custom order.
Each dialog with a user can have a custom "state" which stores data between steps.

### Usage
Pretty straightforward.

```csharp
namespace MyBot
{
    // Step 1: define the state
    public class MyState: StateBase
    {
        public string Name { get; set; }
    }

    // Step 2: define the dialog steps
    public static class RunnerDefinition
    {
        public static Runner<MyState> Create()
        {
            return new RunnerBuilder<MyState>()
                .Step(async s => { await s.Reply("Please enter your name"); })
                .Step(async s =>
                {
                    s.State.Name = s.Message.Text;
                    await s.Reply("Nice to meet you, " + s.State.Name);
                })
                .Build();
        }
    }
    
    // Step 3: run the bot
    class Program
    {
        private const string Token = "...";

        static async Task Main(string[] args)
        {
            var runner = RunnerDefinition.Create();
            var client = new TelegramBotClient(Token); // you may need to add a proxy

            client.OnMessage += async (s, e) => await runner.HandleAsync(client, e.Message);
            client.StartReceiving();

            Thread.Sleep(int.MaxValue);
        }
    }
}
```

### API

When declaring a step, you need to provide the handler in a form of `Func<IExecutionContext<TState>, Task>`.

The `IExecutionContext<TState>` is defined as follows:

```csharp
public interface IExecutionContext<out T> where T: StateBase
{
    /// <summary>
    /// Marks the specified step as next for execution.
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
```

All available Telegram.Bot APIs are available, but you can create shorthand extension methods to make code more conscise. A `Reply` helper is already provided:

```csharp
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
```
