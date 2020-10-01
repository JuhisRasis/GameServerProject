using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameWebApi
{
    [ApiController]
    [Route("players")]
    public class PlayersController : ControllerBase
    {
        private readonly IRepository repo;

        public PlayersController(IRepository _repo)
        {
            repo = _repo;
        }

        [HttpGet("{id:guid}")]
        public Task<Player> Get(Guid id)
        {
            return repo.Get(id);
        }

        public Task<Player[]> GetPlayersWithMinScore([FromQuery] int minScore)
        {
            Console.WriteLine("GetPlayersWithMinScore");
            Console.WriteLine("minScore: " + minScore);
            return repo.GetPlayersWithMinScore(minScore);
        }

        [HttpGet("{name}")]
        public Task<Player> GetPlayerWithName(string name)
        {
            Console.WriteLine("GetPlayersWithName");
            Console.WriteLine("name: " + name);
            return repo.GetPlayerWithName(name);
        }

        public Task<Player[]> GetPlayersWithTag(string tag)
        {
            Console.WriteLine("GetPlayersWithTag");
            Console.WriteLine("name: " + tag);
            return repo.GetPlayersWithTag(tag);
        }


        [HttpGet("top10")]
        public Task<Player[]> GetTop10Players()
        {
            Console.WriteLine("GetTop10Players");
            return repo.GetTop10Players();
        }



        [HttpPost("create")]
        public Task<Player> Create([FromBody] NewPlayer player)
        {
            Player p = new Player();
            p.Name = player.Name;
            p.Id = Guid.NewGuid();
            p.Score = 0;
            p.Level = player.Level;
            p.IsBanned = false;
            p.CreationTime = DateTime.UtcNow;
            return repo.Create(p);
        }

        [HttpPost("modify/{id:guid}")]
        public Task<Player> Modify(Guid id, [FromBody] ModifiedPlayer player)
        {
            // Problem if submitting empty body: updates score to 0 
            return repo.Modify(id, player);
        }

        [HttpPost("update/{id:guid}/name")]
        public Task<Player> UpdatePlayerName(Guid id, [FromQuery] string newName)
        {
            Console.WriteLine("UpdatePlayerName");
            Console.WriteLine("newName: " + newName);
            return repo.UpdatePlayerName(id, newName);
        }

        [HttpPost("update/{id:guid}/incrementScore")]
        public Task<Player> IncrementPlayerScore(Guid id, [FromQuery] int inc)
        {
            Console.WriteLine("IncrementPlayerScore");
            Console.WriteLine("increment: " + inc);
            return repo.IncrementPlayerScore(id, inc);
        }

        [HttpPost("delete/{id:guid}")]
        public Task<Player> Delete(Guid id)
        {
            return repo.Delete(id);
        }

        [HttpOptions]
        public void Options() { }
    }
}