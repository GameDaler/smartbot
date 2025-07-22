using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
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
	public static class PhotoManager
	{
		public const string PhotosPath = "Assets/";

        private static Dictionary<string, byte[]> _photoBytes = new();

        public static async Task InitializeAsync()
        {
            if (!Directory.Exists(PhotosPath))
                Directory.CreateDirectory(PhotosPath);

            var files = Directory.EnumerateFiles(PhotosPath, "*.jpg");

            foreach (var path in files)
            {
                var fileName = Path.GetFileName(path);
                var bytes = await File.ReadAllBytesAsync(path);
                _photoBytes[fileName] = bytes;
            }
        }

		public static async Task<string?> SavePhoto(Message message, TelegramBotClient botClient, CancellationToken token)
		{
			if (message?.Photo != null && message.Photo.Length > 0)
			{
				var fileId = message.Photo.Last().FileId;
				var file = await botClient.GetFile(fileId, cancellationToken: token);

				using var stream = new MemoryStream();
				await botClient.DownloadFile(file.FilePath!, stream, cancellationToken: token);
				stream.Position = 0;

				using var image = await SixLabors.ImageSharp.Image.LoadAsync(stream, token);
				await image.SaveAsJpegAsync(PhotosPath + fileId + ".jpg", new JpegEncoder { Quality = 90 }, token);

				await botClient.SendMessage(
					chatId: message.Chat.Id,
					text: "Фото сохранено как JPG.",
					cancellationToken: token);

				return fileId;
			}
			return null;
		}

        public static InputFileStream? Get(string? fileName)
        {
            if (!_photoBytes.TryGetValue($"{fileName}.jpg", out var bytes))
                return null;

            var stream = new MemoryStream(bytes); // новый каждый раз!
            return new InputFileStream(stream, $"{fileName}.jpg");
        }
        public static List<InputMediaPhoto> GetGroup(string text, params string[] fileNames)
        {
            var photos = new List<InputMediaPhoto>();
            foreach (var fileName in fileNames)
            {
                var photo = Get(fileName);
                if (photo != null)
                {
                    photos.Add(new InputMediaPhoto(photo));
                }
            }
            if (photos.Count > 0)
                photos[0].Caption = text;

            return photos;
        }
        public static List<InputMediaPhoto> GetGroup(string text, List<string> fileNames)
		{
			return GetGroup(text, fileNames.ToArray());
        }
    }
}
