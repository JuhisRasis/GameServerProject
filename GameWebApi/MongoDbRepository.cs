using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;

namespace GameWebApi
{
    // connection string: mongodb://localhost:27017
    public class MongoDbRepository : IRepository
    {
        private IMongoCollection<Player> playersCollection;
        private IMongoCollection<BsonDocument> bsonDocumentCollection;

        public MongoDbRepository()
        {
            MongoClient mongoClient = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase db = mongoClient.GetDatabase("game");
            playersCollection = db.GetCollection<Player>("players");
        }
        public async void AddPlayerInformation(Player player)
        {
            player.Id = Guid.NewGuid();
            player.Score = 1;
            player.IsBanned = false;
            player.CreationTime = DateTime.UtcNow;
            await Create(player);
        }
        public async Task<Player> UpdatePlayerGuessNameNumber(string name, int newGuessGameNumber)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Name, name);
            var guessGameNumberUpdate = Builders<Player>.Update.Set(p => p.GuessGameNumber, newGuessGameNumber);
            var options = new FindOneAndUpdateOptions<Player>()
            {
                ReturnDocument = ReturnDocument.After
            };
            Player player = await playersCollection.FindOneAndUpdateAsync(filter, guessGameNumberUpdate, options);
            return player;
        }


        public async Task<Player> GetPlayerInformation(Player player)
        {

            await GetPlayerWithName(player.Name);
            return player;
        }



        public async Task<Player> Get(Guid id)
        {
            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, id);
            return await playersCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Player[]> GetAll()
        {
            List<Player> p = await playersCollection.Find(new BsonDocument()).ToListAsync();
            return p.ToArray();
        }

        public async Task<Player[]> GetPlayersWithMinScore(int minScore)
        {
            FilterDefinition<Player> filter = Builders<Player>.Filter.Gte(p => p.Score, minScore);
            List<Player> p = await playersCollection.Find(filter).ToListAsync();
            return p.ToArray();
        }

        public async Task<Player> GetPlayerWithName(string name)
        {
            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Name, name);
            Player p = await playersCollection.Find(filter).FirstAsync();
            return p;
        }

        public async Task<Player[]> GetPlayersWithTag(string tag)
        {
            var filter = Builders<Player>.Filter.Eq("Tags", tag);
            List<Player> p = await playersCollection.Find(filter).ToListAsync();
            return p.ToArray();
        }

        public async Task<Player> GetRandomPlayer()
        {
            long i = playersCollection.Find(new BsonDocument()).CountDocuments();
            Random rand = new Random();
            int plrNumber = rand.Next(Convert.ToInt32(i));

            SortDefinition<Player> sortDef = Builders<Player>.Sort.Descending(p => p.Score);
            List<Player> list = await playersCollection.Find(new BsonDocument()).Sort(sortDef).Skip(plrNumber).Limit(1).ToListAsync();
            return list[0];
        }

        public async Task<Player[]> GetTop10Players()
        {
            SortDefinition<Player> sortDef = Builders<Player>.Sort.Descending(p => p.Score);
            List<Player> top10 = await playersCollection.Find(new BsonDocument()).Sort(sortDef).Limit(10).ToListAsync();
            return top10.ToArray();
        }

        public async Task<Player[]> GetlayersWithinThesePostions(int start, int displayAmount)
        {
            SortDefinition<Player> sortDef = Builders<Player>.Sort.Descending(p => p.Score);
            List<Player> top10 = await playersCollection.Find(new BsonDocument()).Sort(sortDef).Skip(start).Limit(displayAmount).ToListAsync();
            return top10.ToArray();

        }
        public async Task<long> GetDocumentAmount()
        {
            long amount = playersCollection.Find(new BsonDocument()).CountDocuments();
            return amount;
        }

        public async Task<Player> Create(Player player)
        {
            await playersCollection.InsertOneAsync(player);
            return player;
        }

        public async Task<Player> Modify(Guid id, ModifiedPlayer player)
        {
            Player p = await Get(id);

            if (p != null)
            {

                if (player.Score != p.Score) // Problem if submitting empty body: updates score to 0 
                {
                    p.Score = player.Score;
                    Console.WriteLine("Changed players: " + id + " score to: " + player.Score);
                }

                FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, id);
                await playersCollection.ReplaceOneAsync(filter, p);
                //Console.WriteLine("Added item " + player.item.ItemType.ToString() + " with id: " + player.item.Id + " to player: " + id);
                return await Get(id);
            }
            else
            {
                Console.WriteLine("Couldnt modify player: Player with id: " + id + " not found!");
                return null;
            }
        }

        public async Task<Player> UpdatePlayerName(Guid id, string newName)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Id, id);
            var nameUpdate = Builders<Player>.Update.Set(p => p.Name, newName);
            var options = new FindOneAndUpdateOptions<Player>()
            {
                ReturnDocument = ReturnDocument.After
            };
            Player p = await playersCollection.FindOneAndUpdateAsync(filter, nameUpdate, options);
            return p;
        }

        public async Task<Player> IncrementPlayerScore(Guid id, int increment)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Id, id);
            var scoreIncrementUpdate = Builders<Player>.Update.Inc(p => p.Score, increment);
            var options = new FindOneAndUpdateOptions<Player>()
            {
                ReturnDocument = ReturnDocument.After
            };
            Player p = await playersCollection.FindOneAndUpdateAsync(filter, scoreIncrementUpdate, options);
            return p;
        }

        public async Task<Player> Delete(Guid id)
        {
            Player deletedPlayer = await Get(id);

            if (deletedPlayer != null)
            {
                FilterDefinition<Player> filter = Builders<Player>.Filter.Eq(p => p.Id, id);
                await playersCollection.DeleteOneAsync(filter);
                Console.WriteLine("Deleted player with id: " + id);
                return deletedPlayer;
            }
            else
            {
                Console.WriteLine("Couldnt delete player: Player with id: " + id + " not found!");
            }

            return null;
        }

    }
}