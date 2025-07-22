using LiteDB;
using LiteDB.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Smart_Bot.JsonData
{
    public class BotDatabase
    {
        private readonly LiteDatabaseAsync _db;
        private readonly ILiteCollectionAsync<BotData> _bots;

        public BotDatabase(string name)
        {
            // Файл БД создастся автоматически
            var path = name.Replace(":", "_") + ".db";
            _db = new LiteDatabaseAsync(path);
            _bots = _db.GetCollection<BotData>("bots");
        }

        public async Task Save(BotData bot)
        {
            await _bots.UpsertAsync(bot);
        }

        public async Task Delete(string token)
        {
            await _bots.DeleteManyAsync(x => x.Token == token);
            Console.WriteLine("DELETED FROM DB");
        }

        public async Task<BotData[]> GetAllBotsAsync()
        {
            var allUsers = await _bots.FindAllAsync();
            return allUsers.ToArray();
        }
    }
}
