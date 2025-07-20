using Smart_Bot.JsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Smart_Bot
{
	public static class UpdateHandler
	{
		public static async Task HandleMessage(Message message, Admin admin, TelegramBotClient botClient, long chatId)
		{
			Console.WriteLine("handle message");
			if ((message.Text == null || message.Text.Length == 0) && admin.CreateState != AdminCreateStates.SendPhoto)  return;

			var botData = admin.SelectedBot;
			Console.WriteLine(admin.CreateState.ToString());
			if (admin.CreateState != AdminCreateStates.None)
			{
				await BotCreator.Handle(botClient, chatId, message, botData, admin);
				return;
			}

			if (botData != null) await BotCreator.Handle(botClient, chatId, message, botData, admin);
			else
			{
				switch (message.Text)
				{
					case "/newbot":
						await BotCreator.OnNewBotCreate(botClient, message.Text, chatId, null, admin);
						break;
					case "/mybots":
						var buttons = new InlineKeyboardMarkup();

						foreach (var bot in Program.BotsList)
						{
							var botUser = await bot.BotClient.GetMe();
							var username = botUser.Username ?? "";

							buttons.AddNewRow(InlineKeyboardButton.WithUrl(botUser.FirstName, "t.me/" + username));
						}

						if (buttons.InlineKeyboard.Count() == 0)
						{
							await botClient.SendMessage(
								chatId: chatId,
								text: "У вас нет ботов!:");
						}


                        await botClient.SendMessage(
							chatId: chatId, 
							text: "Список ваших ботов:",
							replyMarkup: buttons);

						break;
				}
				if (message.Text.StartsWith("/delete"))
				{
					Console.WriteLine("DELETE");

					Console.WriteLine("\n\n");
					foreach (var listBot in Program.BotsList)
					{
						var userBot = await listBot.BotClient.GetMe();
						Console.WriteLine(userBot.Username);
					}
					Console.WriteLine("\n\n");

					var id = message.Text.Substring(8);
					Bot? bot = null;

					bot = await Program.Find(id);

					if (bot == null) return;

					Program.BotsList.Remove(bot);
					await bot.DisconnectBot();

					await Program.BotDb.Delete(bot.Token);

					await Program.botClient.SendMessage(chatId, "Бот удалён!");

					Console.WriteLine("deleted");
				}
			}
		}
	}
}
