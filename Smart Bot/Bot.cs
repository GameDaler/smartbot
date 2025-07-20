using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Smart_Bot
{
	public class Bot
	{
		public TelegramBotClient BotClient;
		public string Text { get; set; }
		public string Token { get; set; }
		public string ChannelUrl { get; set; }
		public string? PhotoId { get; set; }
		public UrlButtonKeyboard UrlButtonKeyboard { get; set; }

		public Bot(string token, string text, string channelUrl, string photoId, UrlButtonKeyboard? urlButtonKeyboard = null)
		{
			BotClient = new TelegramBotClient(token);

			Token = BotClient.Token;
			Text = text;
			ChannelUrl = channelUrl;

			PhotoId = photoId;

			UrlButtonKeyboard = urlButtonKeyboard ?? new UrlButtonKeyboard();

			BotClient.OnMessage += OnMessageAsync;
			BotClient.OnError += HandleErrorAsync;
		}
		async Task OnMessageAsync(Message message, UpdateType updateType)
		{
			Console.WriteLine("IN NEW BOT: " + message.Text);

            Console.WriteLine(UrlButtonKeyboard.urlButtons.Count);

			if (message?.From?.Id == null) return;	
			var chatId = message.From.Id;

			var text = message.Text ?? "";

			if (text.StartsWith("/start"))
			{
				var photo = PhotoManager.Get(PhotoId);
				if (photo == null)
				{
					Console.WriteLine("only text");
					await BotClient.SendMessage(
						chatId: chatId,
						text: Text,
						replyMarkup: UrlButtonKeyboard.ToInlineKeyboard());
				}
				else
				{
					Console.WriteLine("with photo");
					await BotClient.SendPhoto(
						chatId: chatId,
						photo: photo,
						caption: Text,
						replyMarkup: UrlButtonKeyboard.ToInlineKeyboard());

				}
				await BotClient.SendMessage(
					chatId: chatId,
					text: $"Посетите наш канал: {ChannelUrl}",
					linkPreviewOptions: new LinkPreviewOptions { IsDisabled = false });
			}
		}
		static async Task HandleErrorAsync(Exception exception, HandleErrorSource source)
		{
			Console.WriteLine(exception.Message + "\n" + exception.StackTrace);
		}
        public async Task DisconnectBot()
        {
            try
            {
                // 2. Отписываемся от событий
                BotClient.OnMessage -= OnMessageAsync;
                BotClient.OnError -= HandleErrorAsync;

                // 3. Освобождаем ресурсы
                await BotClient.Close();
                BotClient = null;

                Console.WriteLine("Бот успешно отключен");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отключении бота: {ex.Message}");
            }
        }
    }
}
