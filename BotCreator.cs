using Smart_Bot.JsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using static System.Net.Mime.MediaTypeNames;

namespace Smart_Bot
{
	public static class BotCreator
	{
		public static async Task Handle(TelegramBotClient botClient, long chatId, Message message, BotData? bot, Admin admin)
		{
            Console.WriteLine("create");
			Admin? editedAdmin = null;

			var text = message.Text;

			switch (admin.CreateState)
			{
				case AdminCreateStates.SendToken: editedAdmin = await OnTokenSended(botClient, text, chatId, bot, admin); break;
				case AdminCreateStates.SendText: editedAdmin = await OnTextSended(botClient, text, chatId, bot, admin); break;
				case AdminCreateStates.SendPhoto: editedAdmin = await OnPhotoSended(botClient, message, text, chatId, bot, admin); break;
				case AdminCreateStates.SendUrl: editedAdmin = await OnUrlSended(botClient, text, chatId, bot, admin); break;
				case AdminCreateStates.SendButtonText: editedAdmin = await OnButtonTextSended(botClient, text, chatId, bot, admin); break;
				case AdminCreateStates.SendButtonUrl: editedAdmin = await OnButtonUrlSended(botClient, text, chatId, bot, admin); break;
			}

			if (editedAdmin != null)
			{
				await Program.AdminDb.Save(editedAdmin);
			}
		}

		public static async Task OnNewBotCreate(TelegramBotClient botClient, string text, long chatId, BotData? bot, Admin admin)
		{
            Console.WriteLine("newbot");

			await botClient.SendMessage(chatId, "Давайте приступим к созданию нового бота! Пожалуйста, отправьте токен бота, который вы получили от BotFather.");
			admin.CreateState = AdminCreateStates.SendToken;


            await Program.AdminDb.Save(admin);
        }

		public static async Task<Admin?> OnTokenSended(TelegramBotClient botClient, string text, long chatId, BotData bot, Admin admin)
		{
            Console.WriteLine("ontoken");
			if (Program.BotExists(text))
			{
				await botClient.SendMessage(chatId, "Бот с таким токеном уже привязан. Пожалуйста, отправьте другой токен.");
				return null;
			}
			try
			{
				new TelegramBotClient(text);

				await botClient.SendMessage(chatId, "Отлично, нам удалось подключиться к боту!\n\nТеперь нужен текст сообщения для бота.");

				var newBotData = new BotData
				{
					Token = text,
				};

				Program.AddBotInCreateProcess(newBotData);

				admin.CreateState = AdminCreateStates.SendText;
				admin.SelectedBot = new BotData();
				admin.SelectedBot.Token = text;
			}
			catch (Exception ex)
			{
				await botClient.SendMessage(chatId, "Не удалось подключиться к боту. Кажется, такого токена нет.");
				admin.CreateState = AdminCreateStates.None;
				admin.SelectedBot = null;
			}

            return admin;
        }

		public static async Task<Admin?> OnTextSended(TelegramBotClient botClient, string text, long chatId, BotData bot, Admin admin)
		{
			if (bot == null) return null;
			if (text == null || text == "")
			{
                await botClient.SendMessage(chatId, "Ты не можешь отправить пустой текст, похоже ты отправил фото, видео или файл");
            }

			bot.Text = text;
			admin.CreateState = AdminCreateStates.SendPhoto;
			await botClient.SendMessage(chatId, "Отлично, текст сохранён! Теперь отправь фото для сообщения" +
				"\nБот принимает только по одному, но дальше в настройках бота можно будет добавить ещё.\n\nЕсли не хочешь пока добавлять фото, отправь /skip");

            return admin;
        }

		public static async Task<Admin?> OnPhotoSended(TelegramBotClient botClient, Message message, string text, long chatId, BotData bot, Admin admin)
		{
            Console.WriteLine("PHOTO");
			if (bot == null) return null;

			if (text != null && text.StartsWith("/skip"))
			{
				admin.CreateState = AdminCreateStates.SendUrl;
				await botClient.SendMessage(chatId, "Хорошо, потом добавим! Теперь отправь ссылку на твой канал в следующем формате:" +
					"\n\nЕсли отправляешь публичную ссылку, то напиши t.me/@your_channel_name" +
					"\n\nЕсли отправляешь приватную ссылку, то её редактировать не надо, кидай оригинальную ссылку");
			}
			else
			{
				string? photoId = await PhotoManager.SavePhoto(message, botClient, new CancellationToken());
				if (photoId == null)
				{
					await botClient.SendMessage(chatId, "Фото не было обработано, либо оно вообще не было отправлено. Попробуй ещё раз и убедись, что кидашеь фото, а не видео, или файл");
					return null;
				}

				bot.PhotoId = photoId;

				await botClient.SendMessage(chatId, $"Фото {photoId} сохранено! Теперь отправь ссылку на твой канал в следующем формате:" +
					"\n\nЕсли отправляешь публичную ссылку, то напиши t.me/@your_channel_name" +
					"\n\nЕсли отправляешь приватную ссылку, то её редактировать не надо, кидай оригинальную ссылку");

				admin.CreateState = AdminCreateStates.SendUrl;
			}

            return admin;
        }

		public static async Task<Admin?> OnUrlSended(TelegramBotClient botClient, string text, long chatId, BotData bot, Admin admin)
		{
			if (bot == null) return null;

			bot.ChannelUrl = text;

			admin.CreateState = AdminCreateStates.SendButtonText;

			await botClient.SendMessage(chatId, $"Канал {text} был сохранён! Теперь добавим кнопки к сообщению. Сначала отправляй текст кнопки, потом ссылку для кнопки.");
			await botClient.SendMessage(chatId, $"Отправь текст для кнопки.\n\nНа этом этапе ты можешь написать /stop и мы прекратим добавалять кнопки.");

            return admin;
        }

		public static async Task<Admin?> OnButtonTextSended(TelegramBotClient botClient, string text, long chatId, BotData bot, Admin admin)
		{
			var botDb = Program.BotDb;

			if (bot == null) return null;

			if (text.StartsWith("/stop"))
			{
				await botDb.Save(bot);

				Console.WriteLine("\n\nNEW BOT INFO\n");
				Console.WriteLine($"Token:{bot.Token}\nText:{bot.Text}\nPhoto:{bot.PhotoId}\nUrl:{bot.ChannelUrl}");
				foreach (var button in bot.UrlButtonKeyboard.urlButtons)
				{
					Console.WriteLine($"Url Button:\n	Text:{button.Text}\n	Url{button.CallbackData}");
				}

				var newBot = Program.InitializeBot(bot);

				admin.CreateState = AdminCreateStates.None;
				admin.SelectedBot = null;
				admin.EditableButton = null;

				await botClient.SendMessage(chatId, $"Настройка бота завершена! Теперь можешь протестировать его {await newBot.BotClient.GetMe()}, либо посмотреть список своих ботов /my_bots");
				return admin;
			}

			admin.EditableButton = new UrlButton(text, "");
			admin.CreateState = AdminCreateStates.SendButtonUrl;

			await botClient.SendMessage(chatId, $"Отправь ссылку для кнопки.");

            return admin;
        }

		public static async Task<Admin?> OnButtonUrlSended(TelegramBotClient botClient, string text, long chatId, BotData bot, Admin admin)
		{
			var botDb = Program.BotDb;

			if (bot == null) return null;
			if (admin.EditableButton == null) admin.EditableButton = new UrlButton("", "");

			admin.EditableButton.CallbackData = text;
			bot.UrlButtonKeyboard.Add(admin.EditableButton);

			admin.CreateState = AdminCreateStates.SendButtonText;

			await botClient.SendMessage(chatId, $"Отправь текст для кнопки.\n\nНа этом этапе ты можешь написать /stop и мы прекратим добавалять кнопки.");

			return admin;
		}
	}
}
