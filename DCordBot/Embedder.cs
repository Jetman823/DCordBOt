using Discord;
using System;

namespace DCordBot
{
    class Embedder : EmbedBuilder
    {
        public Embedder()
        {
            Color = Discord.Color.Red;
        }

        public void SetAuthor(EmbedAuthorBuilder author)
        {
            Author = author;
        }
        
        public void SetColor(Color color)
        {
            Color = color;
        }

        public void SetTimeStamp(DateTimeOffset timeOffset)
        {
            Timestamp = timeOffset;
        }

        public void InsertField(EmbedFieldBuilder embedField)
        {
            AddField(embedField);
        }

        public void AddImageUrl(string imageurl)
        {
            ImageUrl = imageurl;
        }

        public void AddUrl(string url)
        {
            Url = url;
        }
        
        public void SetDescription(string desc)
        {
            Description = desc;
        }

        public void SetTitle(string title)
        {
            Title = title;
        }

        public void AddEmbedFooter(EmbedFooterBuilder footerBuilder)
        {
            Footer = footerBuilder;
        }

        public void AddThumbNailUrl(string url)
        {
            ThumbnailUrl = url;
        }
    }
}
