using System.Threading;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

// Загружаем фото в память (например, при старте бота)
using var stream = File.OpenRead("photo.jpg");
var photo = new InputFileStream(stream, "photo.jpg");

TelegramBotClient botClient = new TelegramBotClient("7678536168:AAGeITsfuERSXTLxqLJDlS99G4kve1AU-KI");
CancellationTokenSource cts = new CancellationTokenSource();

botClient.OnMessage += OnMessageAsync;
botClient.OnError += HandleErrorAsync;

// Получаем информацию о боте
var me = await botClient.GetMe();

Console.WriteLine($"Бот @{me.Username} запущен!");
Console.WriteLine("Нажмите любую клавишу для остановки...1");
Console.ReadKey();


// Остановка бота
cts.Cancel();

async Task OnMessageAsync(Message message, UpdateType updateType)
{
	var chatId = message.From.Id;
	if (message.Text == "/start")
	{
		await botClient.SendPhoto(
			chatId: chatId,
            photo: photo,
            caption: "🔞 СОБРАЛИ НЕСКОЛЬКО АРХИВОВ ДЛЯ ВАС:" +
			"\n- АЛЬТУШКИ\n- ДОМАШНИЕ СЛИВЫ\n- НЯШКИ\n- СТУДЕНТКИ" +
			"\n- БЫВШИЕ \n\nДОСТУП 🔞👇\nhttps://t.me/+Zc0JAA3yeBlhZDZk" +
			"\nhttps://t.me/+Zc0JAA3yeBlhZDZk\r\nhttps://t.me/+Zc0JAA3yeBlhZDZk",
			replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("🔞ПРОБНИК🔞", "https://t.me/+Zc0JAA3yeBlhZDZk")));

		await botClient.SendMessage(
			chatId: chatId,
			text: "Посетите наш канал: https://t.me/+_hwnuAD--CBiNjNk",
			linkPreviewOptions: new LinkPreviewOptions { IsDisabled = false }
		);
	}
}

async Task HandleErrorAsync(Exception exception, HandleErrorSource source)
{
	Console.WriteLine(exception.Message);
}
	 
