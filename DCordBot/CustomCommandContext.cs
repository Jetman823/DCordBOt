using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DCordBot
{
    /// <summary>
    /// TODO: build a custom context so i can get arg counts
    /// </summary>
    public class CustomCommandContext : ICommandContext
    {
        public IDiscordClient Client => throw new System.NotImplementedException();

        public IGuild Guild => throw new System.NotImplementedException();

        public IMessageChannel Channel => throw new System.NotImplementedException();

        public IUser User => throw new System.NotImplementedException();

        public IUserMessage Message => throw new System.NotImplementedException();
    }
}
