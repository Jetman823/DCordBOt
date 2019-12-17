using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Xml;



namespace DCordBot
{
    class Program
    {
        public static SqlConnection connection = null;
        public static SqlConnection botConnection = null;
        public static DiscordSocketClient client;
        public CommandModule commandModule;
        public ZItemManager itemManager;
        public static NFSW nFSWImages = new NFSW();
        public Program()
        {
            //connection = new SqlConnection(@"Server=WIN-IUIQ9BTIQRM\SQLEXPRESS;Database=GunzDB2;Trusted_Connection=Yes;");
            //try
            //{
            //    connection.Open();
            //}
            //catch (SqlException exception)
            //{
            //    Console.Write(exception.Message);
            //}
            botConnection = new SqlConnection(@"Server=DESKTOP-AR9BK07;Database=DCordBot;Trusted_Connection=Yes;");
            try
            {
                botConnection.Open();
            }
            catch(SqlException ex)
            {
                Console.Write(ex.Message);
            }

            client = new DiscordSocketClient();
            client.Ready += ReadyAsync;
            client.MessageReceived += MessageReceieved;
            client.UserJoined += UserJoined;
            client.UserLeft += UserLeft;
            itemManager = new ZItemManager();
            itemManager.Load();

            nFSWImages.Load();
            
        }

        public static void Main(string[] args)
        {
            new Program().StartBot().GetAwaiter().GetResult();

        }


        public async Task StartBot()
        {
            commandModule = new CommandModule();
            commandModule.Load();

            await client.LoginAsync(Discord.TokenType.Bot, "NDM2Njg2NjAzMzk4OTM4NjQ0.XfQO1g.bf5lego0eEexHKHFKygYEfFmRJc", true);
            await client.StartAsync();
            await Task.Delay(-1);

        }
    

        public Task ReadyAsync()
        {
            Console.WriteLine("Ready to connect");
            return Task.CompletedTask;
        }

        private async Task MessageReceieved(SocketMessage message)
        {
            //if (connection.State != System.Data.ConnectionState.Open)
            //    return;
            ///bot doesn't need to respond to  a bot
            if (message.Author.IsBot)
                return;

            if (message.Content[0] == '!')
            {
                string lowerCaseMsg = message.Content.ToLower();

                foreach (CommandInfo command in commandModule.commandList)
                {
                    string commandName = command.commandName.ToLower();

                    if (lowerCaseMsg.Contains(commandName))
                    { 

                        Console.WriteLine("command found");
                        await commandModule.Response(message, command);
                    }
                }
            }
        }

        /// <summary>
        /// Need to write this
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task UserJoined(SocketGuildUser user)
        {
            ITextChannel channel = user.Guild.DefaultChannel;
            var embed = new EmbedBuilder
            {
                ImageUrl = "https://uploads.disquscdn.com/images/4df0100942caa8c7688300d788f69fbe905d041d1969fe51b9442caca6f88be8.gif",
                Color = Color.Red,
                Description = $"{user.Username} has joined the server!"
            };
            await channel.SendMessageAsync("", false, embed.Build());
        }

        private async Task UserLeft(SocketGuildUser user)
        {
            ITextChannel channel = user.Guild.DefaultChannel;
            var embed = new EmbedBuilder
            {
                ImageUrl = "http://pa1.narvii.com/5994/b140573f8431754feb055d6e592321cc13b53b14_00.gif",
                Color = Color.Red,
                Description = $"{user.Username} has left us!"
            };

            await channel.SendMessageAsync("", false, embed.Build());
        }
    }
}

