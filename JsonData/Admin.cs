using LiteDB.Async;
using SixLabors.ImageSharp.Processing.Processors.Normalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Smart_Bot.JsonData
{
	public class AdminDB
	{
		private readonly LiteDatabaseAsync _db;
		private readonly ILiteCollectionAsync<Admin> _admins;

		public AdminDB(string name)
		{
			// Файл БД создастся автоматически
			var path = name.Replace(":", "_") + ".db";
			_db = new LiteDatabaseAsync(path);
			_admins = _db.GetCollection<Admin>("admins");
			_admins.EnsureIndexAsync(x => x.Id);
		}

		public async Task Save(Admin admin)
		{
            await _admins.UpsertAsync(admin);
        }

		public async Task InitializeDB()
		{
            var allUsers = await _admins.FindAllAsync();
            foreach (var user in allUsers)
            {
                user.CreateState = AdminCreateStates.None;
				user.SelectedBot = null;
				await _admins.UpdateAsync(user);
            }
        }


        public async Task<Admin?> Find(long id)
		{
            return await _admins.FindOneAsync(x => x.Id == id);
		}
	}
	public class Admin
	{
		public long Id { get; set; }
		
		public AdminCreateStates CreateState { get; set; } = AdminCreateStates.None;
		public BotData? SelectedBot { get; set; }
		public UrlButton? EditableButton { get; set; }

		public Admin(long id)
		{
			Id = id;
        }
	}
	public enum AdminCreateStates
	{
		None,
		SendToken,
		SendText,
		SendPhoto,
		SendUrl,
		SendButtonText,
		SendButtonUrl
    }
}