using System;
using System.Threading.Tasks;

namespace GameWebApi
{
    public interface IRepository
    {
        Task<Player> Get(Guid id);
        Task<Player[]> GetAll();
        Task<Player[]> GetPlayersWithMinScore(int minScore);
        Task<Player> GetPlayerWithName(string name);
        Task<Player[]> GetPlayersWithTag(string tag);
        Task<Player[]> GetTop10Players();
        Task<Player> Create(Player player);
        Task<Player> Modify(Guid id, ModifiedPlayer player);
        Task<Player> UpdatePlayerName(Guid id, string newName);
        Task<Player> IncrementPlayerScore(Guid id, int increment);
        Task<Player> Delete(Guid id);

    }
}