using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DCordBot
{
    class DiscordClientWrapper : IDiscordClientWrapper, IDisposable
    {
        private static readonly MethodInfo _disconnectMethod = typeof(SocketGuild)
            .GetMethod("DisconnectAudioAsync", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly BaseSocketClient _baseSocketClient;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscordClientWrapper"/> class.
        /// </summary>
        /// <param name="client">the sharded discord client</param>
        public DiscordClientWrapper(DiscordShardedClient client)
            : this(client, client.Shards.Count)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscordClientWrapper"/> class.
        /// </summary>
        /// <param name="client">the sharded discord client</param>
        /// <param name="shards">the number of total shards</param>
        public DiscordClientWrapper(DiscordShardedClient client, int shards)
            : this(client as BaseSocketClient, shards)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscordClientWrapper"/> class.
        /// </summary>
        /// <param name="client">the sharded discord client</param>
        public DiscordClientWrapper(DiscordSocketClient client)
            : this(client, 1)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DiscordClientWrapper"/> class.
        /// </summary>
        /// <param name="baseSocketClient">the discord client</param>
        /// <param name="shards">the number of shards</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="baseSocketClient"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     thrown if the specified shard count is less than 1.
        /// </exception>
        public DiscordClientWrapper(BaseSocketClient baseSocketClient, int shards)
        {
            if (shards < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(shards), shards, "Shard count must be at least 1.");
            }

            _baseSocketClient = baseSocketClient ?? throw new ArgumentNullException(nameof(baseSocketClient));
            _baseSocketClient.VoiceServerUpdated += OnVoiceServerUpdated;
            _baseSocketClient.UserVoiceStateUpdated += OnVoiceStateUpdated;

            ShardCount = shards;
        }

        /// <summary>
        ///     An asynchronous event which is triggered when the voice server was updated.
        /// </summary>
        public event AsyncEventHandler<VoiceServer> VoiceServerUpdated;

        /// <summary>
        ///     An asynchronous event which is triggered when a user voice state was updated.
        /// </summary>
        public event AsyncEventHandler<VoiceStateUpdateEventArgs> VoiceStateUpdated;

        /// <summary>
        ///     Gets the current user snowflake identifier value.
        /// </summary>
        public ulong CurrentUserId => _baseSocketClient.CurrentUser.Id;

        /// <summary>
        ///     Gets the number of total shards the bot uses.
        /// </summary>
        public int ShardCount { get; }

        /// <summary>
        ///     Disposes the wrapper and unregisters all events attached to the discord client.
        /// </summary>
        public void Dispose()
        {
            _baseSocketClient.VoiceServerUpdated -= OnVoiceServerUpdated;
            _baseSocketClient.UserVoiceStateUpdated -= OnVoiceStateUpdated;
        }

        /// <summary>
        ///     Gets the snowflake identifier values of the users in the voice channel specified by
        ///     <paramref name="voiceChannelId"/> (the snowflake identifier of the voice channel).
        /// </summary>
        /// <param name="guildId">the guild identifier snowflake where the channel is in</param>
        /// <param name="voiceChannelId">the snowflake identifier of the voice channel</param>
        /// <returns>
        ///     a task that represents the asynchronous operation
        ///     <para>the snowflake identifier values of the users in the voice channel</para>
        /// </returns>
        public Task<IEnumerable<ulong>> GetChannelUsersAsync(ulong guildId, ulong voiceChannelId)
        {
            var guild = _baseSocketClient.GetGuild(guildId)
               ?? throw new ArgumentException("Invalid or inaccessible guild: " + guildId, nameof(guildId));

            var channel = guild.GetVoiceChannel(voiceChannelId)
                ?? throw new ArgumentException("Invalid or inaccessible voice channel: " + voiceChannelId, nameof(voiceChannelId));

            return Task.FromResult(channel.Users.Select(s => s.Id));
        }

        /// <summary>
        ///     Awaits the initialization of the discord client asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation</returns>
        public async Task InitializeAsync()
        {
            var startTime = DateTimeOffset.UtcNow;

            // await until current user arrived
            while (_baseSocketClient.CurrentUser is null)
            {
                await Task.Delay(10);

                // timeout exceeded
                if (DateTimeOffset.UtcNow - startTime > TimeSpan.FromSeconds(10))
                {
                    throw new TimeoutException("Waited 10 seconds for current user to arrive! Make sure you start " +
                        "the discord client, before initializing the discord wrapper!");
                }
            }
        }

        /// <summary>
        ///     Sends a voice channel state update asynchronously.
        /// </summary>
        /// <param name="guildId">the guild snowflake identifier</param>
        /// <param name="voiceChannelId">
        ///     the snowflake identifier of the voice channel to join (if <see langword="null"/> the
        ///     client should disconnect from the voice channel).
        /// </param>
        /// <param name="selfDeaf">a value indicating whether the bot user should be self deafened</param>
        /// <param name="selfMute">a value indicating whether the bot user should be self muted</param>
        /// <returns>a task that represents the asynchronous operation</returns>
        public Task SendVoiceUpdateAsync(ulong guildId, ulong? voiceChannelId, bool selfDeaf = false, bool selfMute = false)
        {
            var guild = _baseSocketClient.GetGuild(guildId)
                ?? throw new ArgumentException("Invalid or inaccessible guild: " + guildId, nameof(guildId));

            if (voiceChannelId.HasValue)
            {
                var channel = guild.GetVoiceChannel(voiceChannelId.Value)
                    ?? throw new ArgumentException("Invalid or inaccessible voice channel: " + voiceChannelId, nameof(voiceChannelId));

                return channel.ConnectAsync(selfDeaf, selfMute, external: true);
            }

            return (Task)_disconnectMethod.Invoke(guild, new object[0]);
        }

        private Task OnVoiceServerUpdated(SocketVoiceServer voiceServer)
        {
            var args = new VoiceServer(voiceServer.Guild.Id, voiceServer.Token, voiceServer.Endpoint);
            return VoiceServerUpdated.InvokeAsync(this, args);
        }

        private Task OnVoiceStateUpdated(SocketUser user, SocketVoiceState oldSocketVoiceState, SocketVoiceState socketVoiceState)
        {
            // create voice states
            var oldVoiceState = oldSocketVoiceState.VoiceChannel is null ? null : new VoiceState(
                oldSocketVoiceState.VoiceChannel.Id, oldSocketVoiceState.VoiceChannel.Guild.Id, oldSocketVoiceState.VoiceSessionId);

            var voiceState = socketVoiceState.VoiceChannel is null ? null : new VoiceState(
                socketVoiceState.VoiceChannel.Id, socketVoiceState.VoiceChannel.Guild.Id, socketVoiceState.VoiceSessionId);

            // invoke event
            return VoiceStateUpdated.InvokeAsync(this, new VoiceStateUpdateEventArgs(user.Id, voiceState, oldVoiceState));
        }
    }
}
