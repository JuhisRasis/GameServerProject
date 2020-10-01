using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameWebApi.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayersController : ControllerBase
    {
        private readonly ILogger<PlayersController> _logger;
        private MongoDbRepository _repo;
        public PlayersController(ILogger<PlayersController> logger, MongoDbRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        [HttpGet("Get/{id:Guid}")]
        public async Task<Player> Get(Guid id)
        {
            return await _repo.Get(id);
        }

        [HttpGet("GetAll")]
        public async Task<Player[]> GetAll()
        {
            return await _repo.GetAll();
        }


        [HttpGet("Delete/{id:Guid}")]
        public async Task<Player> Delete(Guid id)
        {
            return await _repo.Delete(id);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<Player> Create(NewPlayer newPlayer)
        {
            var player = new Player()
            {
                Id = Guid.NewGuid(),
                Name = newPlayer.Name,
                CreationTime = DateTime.UtcNow
            };
            return await _repo.Create(player);
        }

    }
}