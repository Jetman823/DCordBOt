using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using static DCordBot.CustomPreconditions;

public enum Cards
{
    card_ace = 1,
    card_two = 2,
    card_three = 3,
    card_four = 4,
    card_five = 5,
    card_six = 6,
    card_seven = 7,
    card_eight = 8,
    card_nine = 9,
    card_ten = 10,
    card_jack = 10,
    card_queen = 10,
    card_king = 10
};

public enum Suits
{
    suit_clubs = 1,
    suit_spades = 2,
    suit_hearts = 3,
    suit_diamons = 4
}

public class BJPlayers
{
    public ulong userID;
    public int playerScore;
    public int botScore;

    public BJPlayers(ulong id, int v1, int v2)
    {
        this.userID = id;
        this.playerScore = v1;
        this.botScore = v2;
    }
}

namespace DCordBot
{
    public class BlackJack : CommandHandler
    {
        public static Dictionary<ulong, List<BJPlayers>> bjPlayers = new Dictionary<ulong, List<BJPlayers>>();

        //TODO: finish this
        [Command("bjnew")]
        [Summary("Starts a new blackjack game")]
        [CheckBJUser]
        public async Task ResponseBjNew()
        {


            if (bjPlayers[Context.Guild.Id].Select(x => x.userID == Context.Message.Author.Id) == null)
            {
                var player = bjPlayers[Context.Guild.Id].Find(x => x.userID == Context.Message.Author.Id);

                BJPlayers bjPlayer = new BJPlayers(Context.Message.Author.Id, 0, 0);
                bjPlayers[Context.Guild.Id].Add(bjPlayer);
            }
        }

        [Command("bjhit")]
        [CheckBJUser]
        public async Task ResponseBjHit()
        {
            if (bjPlayers[Context.Guild.Id].Select(x => x.userID == Context.Message.Author.Id) == null)
                return;

            BJPlayers player = bjPlayers[Context.Guild.Id].Find(x => x.userID == Context.Message.Author.Id);
            int playerCard = 5;
            int botCard = 4;
            player.playerScore += playerCard;
            player.botScore += botCard;


            string score = "Your Score: " + player.playerScore + " Bot Score: " + player.botScore;
            await Context.Channel.SendMessageAsync(score);

            if (player.playerScore > 21 && player.botScore <= 21)
                await ResponseBjLose(Context);
            else if (player.playerScore <= 21 && player.botScore > 21)
                await ResponseBjWin(Context);

        }

        [Command("bjstay")]
        [CheckBJUser]
        private async Task ResponseBjStay(SocketMessage message, SocketUser sender)
        {


            BJPlayers player = bjPlayers[Context.Guild.Id].Find(x => x.userID == Context.Message.Author.Id);
            int botCard = 4;
            player.botScore += botCard;


            string score = "Your Score: " + player.playerScore + " Bot Score: " + player.botScore;
            await Context.Channel.SendMessageAsync(score);

            if (player.playerScore > 21 && player.botScore <= 21)
                await ResponseBjLose(Context);
            else if (player.playerScore <= 21 && player.botScore > 21)
                await ResponseBjWin(Context);
        }

        private async Task ResponseBjWin(SocketCommandContext Context)
        {
            bjPlayers[Context.Guild.Id].Remove(bjPlayers[Context.Guild.Id].Single(la => la.userID == Context.Message.Author.Id));

            Embedder embedder = new Embedder();
            embedder.AddThumbNailUrl(Context.Message.Author.GetAvatarUrl());
            embedder.SetDescription("YOU WIN! Your current score is");


            await Context.Channel.SendMessageAsync("",false,embedder.Build());
        }

        private async Task ResponseBjLose(SocketCommandContext Context)
        {
            await Context.Channel.SendMessageAsync("You LOSE!!!!!");


            bjPlayers[Context.Guild.Id].Remove(bjPlayers[Context.Guild.Id].Single(la => la.userID == Context.Message.Author.Id));

        }

    }
}