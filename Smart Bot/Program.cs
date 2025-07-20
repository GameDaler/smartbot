using System.Threading;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Smart_Bot.JsonData;

namespace Smart_Bot
{
	static class Program
	{
		static List<BotData> BotsInCreateProcess = new List<BotData>();
		public static List<Bot> BotsList = new List<Bot>();

		public static AdminDB AdminDb = new AdminDB("admins");
		public static BotDatabase BotDb = new BotDatabase("bots");

		public static TelegramBotClient botClient = new TelegramBotClient("7678536168:AAGeITsfuERSXTLxqLJDlS99G4kve1AU-KI");
		static CancellationTokenSource cts = new CancellationTokenSource();

		static async Task Main(string[] args)
		{
			botClient.OnUpdate += OnUpdateAsync;
			botClient.OnError += HandleErrorAsync;

			await PhotoManager.InitializeAsync();
			await AdminDb.InitializeDB();

			await InitializeBots();

			long authorId = 1747192586;
			var author = await AdminDb.Find(authorId);

			if (author == null)
				await AdminDb.Save(new Admin(authorId));

			var me = await botClient.GetMe();

			Console.WriteLine($"Бот @{me.Username} запущен!");
			Console.WriteLine("Нажмите любую клавишу для остановки...");

			await Task.Delay(-1);

			// Остановка бота
			cts.Cancel();
		}

		public static async Task InitializeBots()
		{
			foreach (var bot in await BotDb.GetAllBotsAsync())
			{
				var newBot = new Bot(
					token: bot.Token,
					text: bot.Text,
					channelUrl: bot.ChannelUrl,
					photoId: bot.PhotoId,
					urlButtonKeyboard: bot.UrlButtonKeyboard);

				BotsList.Add(newBot);
			}
		}
		
		static async Task OnUpdateAsync(Update update)
		{
			if (update.Message is Message message)
			{
				if (message.From?.Id == null) return;

				var text = message.Text ?? "";
				var chatId = message.From.Id;

				Console.WriteLine(message.Text + " " + message.From.Id);

				var admin = await AdminDb.Find(message.From.Id);

				Console.WriteLine(admin == null);

				if (admin != null)
				{
					await UpdateHandler.HandleMessage(message, admin, botClient, chatId);
				}
			}
		}

		static async Task HandleErrorAsync(Exception exception, HandleErrorSource source)
		{
			Console.WriteLine(exception.Message + "\n" + exception.StackTrace + exception.InnerException);
		}

		public static bool BotExists(string token)
		{
			return BotsInCreateProcess.Any(bot => bot.Token == token);
		}

		public static void AddBotInCreateProcess(BotData bot)
		{
			BotsInCreateProcess.Add(bot);
		}

		public static Bot InitializeBot(BotData bot)
		{
			var newBot = bot.ToBot();
			BotsList.Add(newBot);

			return newBot;
		}

		static BotData? FindInCreateProcess(string token)
		{
			foreach(BotData bot in BotsInCreateProcess)
			{
				if (bot.Token == token) return bot;
			}
			return null;
		}

        public static async Task<Bot?> Find(string id)
        {
            foreach (Bot bot in BotsList)
            {
				var userBot = await bot.BotClient.GetMe();
                Console.WriteLine("@" + userBot.Username + " " + id);
                if ("@" + userBot.Username == id) return bot;
            }
            return null;
        }
    }
}
