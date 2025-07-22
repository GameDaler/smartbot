using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart_Bot.JsonData
{
	public class BotData
	{
        public string Token { get; set; }
        public string Text { get; set; }
		public string ChannelUrl { get; set; }
        public string PhotoId { get; set; }
		public UrlButtonKeyboard UrlButtonKeyboard { get; set; } = new UrlButtonKeyboard();

		public BotData(string token, string text, string channelUrl, string photoId, UrlButtonKeyboard urlButtonKeyboard)
		{
			Token = token;
            Text = text;
			ChannelUrl = channelUrl;
            PhotoId = photoId;
			UrlButtonKeyboard = urlButtonKeyboard;
        }
		public BotData() { }

		public Bot ToBot()
		{
			return new Bot(Token, Text, ChannelUrl, PhotoId, UrlButtonKeyboard);
        }
    }
}
