using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Driver;

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

        public async Task<Player[]> GetTop10Players()
        {
            SortDefinition<Player> sortDef = Builders<Player>.Sort.Descending(p => p.Score);
            List<Player> top10 = await playersCollection.Find(new BsonDocument()).Sort(sortDef).Limit(10).ToListAsync();
            return top10.ToArray();
        }


        public async Task<Player> Create(Player player)
        {
            await playersCollection.InsertOneAsync(player);
            Console.WriteLine("Created player with id: " + player.Id);
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