using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;

namespace DCordBot
{
    class Program
    { 
        public static CommandService commands = null;
        private IServiceProvider services = null;
        public static SocketCommandContext context = null;
        public static SqlConnection botConnection = null;
        public static DiscordSocketClient client;
        public static NFSW nFSWImages = new NFSW();
        public static Dictionary<ulong, List<ulong>> guildPlayers = null;
#if GUNZ
        public ZItemManager itemManager;
        public static SqlConnection connection = null;
#endif

        public Program()
        {
#if GUNZ
            connection = new SqlConnection(@"Server=DESKTOP-TMJAO33\SQLEXPRESS;Database=GunZDB15;Trusted_Connection=Yes;");
            try
            {
                connection.Open();
            }
            catch (SqlException exception)
            {
                Console.Write(exception.Message);
            }
#endif
            botConnection = new SqlConnection(@"Server=DESKTOP-1E2U1VN\SQLEXPRESS;Database=DCordBot;Trusted_Connection=Yes;");
            try
            {
                botConnection.Open();
            }
            catch (SqlException ex)
            {
                Console.Write(ex.Message);
            }

            client = new DiscordSocketClient();
            client.Ready += ReadyAsync;
            client.UserJoined += UserJoined;
            client.UserLeft += UserLeft;
            client.JoinedGuild += JoinedGuild;
            client.LeftGuild += LeftGuild;
#if GUNZ
            itemManager = new ZItemManager();
            itemManager.Load();
#endif

            nFSWImages.Load();
        }

        public static void Main(string[] args)
        {
            new Program().StartBot().GetAwaiter().GetResult();
        }


        public async Task StartBot()
        {
            commands = new CommandService();


            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .BuildServiceProvider();
            await RegisterCommandsAsync();
            await client.LoginAsync(TokenType.Bot, "NDM2Njg2NjAzMzk4OTM4NjQ0.XfQO1g.bf5lego0eEexHKHFKygYEfFmRJc", true);
            await client.StartAsync();
            await LoadGuildData();

            await Task.Delay(-1);

        }

        public async Task RegisterCommandsAsync()
        {
            client.MessageReceived += MessageReceieved;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(),services);


            commands.CommandExecuted += CommandExecuted;
        }

        public async Task CommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if(result.IsSuccess == false)
            {
                string response = $"Here's how to use {command.Value.Name}\n";
                response += result.ErrorReason;

                await context.Channel.SendMessageAsync(response);
            }
        }


        public Task ReadyAsync()
        {
            Console.WriteLine("Ready to connect");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// TODO: write a way to check command  output
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task MessageReceieved(SocketMessage message)
        {
#if GUNZ
            if (connection.State != System.Data.ConnectionState.Open)
                return;
#endif

            SocketUserMessage userMessage = message as SocketUserMessage;
            if (userMessage.Author.IsBot || userMessage == null)
                return;

            int argPos = 0;
            if (userMessage.HasStringPrefix("!", ref argPos) ||
                userMessage.HasMentionPrefix(client.CurrentUser, ref argPos))
            { 
                context = new SocketCommandContext(client, message as SocketUserMessage);
                await commands.ExecuteAsync(context, 1, services);
           }
        }
        
        private async Task UserJoined(SocketGuildUser user)
        {

            SqlBuilder builder = new SqlBuilder("spInsertUser", System.Data.CommandType.StoredProcedure);
            builder.AddParameter("@ServerID", System.Data.SqlDbType.BigInt, (long)user.Guild.Id);
            builder.AddParameter("@UserID", System.Data.SqlDbType.BigInt, (long)user.Id);
            await builder.ExecuteNonQueryAsync();

            ITextChannel channel = user.Guild.DefaultChannel;
            Embedder embedder = new Embedder();
            embedder.AddImageUrl("https://uploads.disquscdn.com/images/4df0100942caa8c7688300d788f69fbe905d041d1969fe51b9442caca6f88be8.gif");
            embedder.SetDescription($"{user.Username} has joined the server!");
            embedder.AddThumbNailUrl(user.GetAvatarUrl());

            await channel.SendMessageAsync("", false, embedder.Build());
        }

        private async Task UserLeft(SocketGuildUser user)
        {
            SqlBuilder builder = new SqlBuilder("spRemoveUser", System.Data.CommandType.StoredProcedure);
            builder.AddParameter("@ServerID", System.Data.SqlDbType.BigInt, (long)user.Guild.Id);
            builder.AddParameter("@UserID", System.Data.SqlDbType.BigInt, (long)user.Id);
            await builder.ExecuteNonQueryAsync();

            ITextChannel channel = user.Guild.DefaultChannel;
            Embedder embedder = new Embedder();
            embedder.AddImageUrl("http://pa1.narvii.com/5994/b140573f8431754feb055d6e592321cc13b53b14_00.gif");
            embedder.SetDescription($"{user.Username} has left us!");
            embedder.AddThumbNailUrl(user.GetAvatarUrl());

            await channel.SendMessageAsync("", false, embedder.Build());
        }

        private async Task JoinedGuild(SocketGuild guild)
        {
            SqlBuilder builder = new SqlBuilder("spInsertServer", System.Data.CommandType.StoredProcedure);
            builder.AddParameter("@ServerID", System.Data.SqlDbType.BigInt, (long)guild.Id);
            await builder.ExecuteNonQueryAsync();
        }

        private async Task LeftGuild(SocketGuild guild)
        {
            SqlBuilder builder = new SqlBuilder("spRemoveServer", System.Data.CommandType.StoredProcedure);
            builder.AddParameter("@ServerID", System.Data.SqlDbType.BigInt, (long)guild.Id);
            await builder.ExecuteNonQueryAsync();
        }

        private async Task LoadGuildData()
        {
            return;
            guildPlayers = new Dictionary<ulong, List<ulong>>();
            using (SqlBuilder builder = new SqlBuilder("spFetchGuilds",System.Data.CommandType.StoredProcedure))
            {
                SqlDataReader reader = await builder.ExecuteReader();
                while(await reader.ReadAsync())
                {
                    ulong GuildID = Convert.ToUInt64(reader.GetInt64(0));

                    using(SqlBuilder builder1 = new SqlBuilder("spFetchGuildPlayers",System.Data.CommandType.StoredProcedure))
                    {
                        List<ulong> playerIDS = new List<ulong>();

                        builder1.AddParameter("@ServerID", System.Data.SqlDbType.BigInt, Convert.ToInt64(GuildID));
                        SqlDataReader reader1 = await builder1.ExecuteReader();
                        while (await reader1.ReadAsync())
                        {
                            ulong playerID = Convert.ToUInt64(reader1.GetInt64(0));
                            playerIDS.Add(playerID);
                        }
                        guildPlayers.Add(GuildID, playerIDS);
                    }
                }
            }
        }
    }
}

