using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameWebApi
{
    [ApiController]
    [Route("players")]
    public class ItemsController : ControllerBase
    {

        private readonly IRepository repo;

        public ItemsController(IRepository _repo)
        {
            repo = _repo;
        }

        [HttpGet("{playerId:guid}/items/{itemId:guid}")]
        public Task<Item> GetItem(Guid playerId, Guid itemId)
        {
            Console.WriteLine("HttpGet: GetItem");
            return repo.GetItem(playerId, itemId);
        }

        [HttpGet("{playerId:guid}/items")]
        public Task<Item[]> GetAllItems(Guid playerId)
        {
            Console.WriteLine("HttpGet: GetAllItems");
            return repo.GetAllItems(playerId);
        }

        [NotAllowedExceptionFilter]
        [HttpPost("{playerId:guid}/items/create")]
        public async Task<Item> CreateItem(Guid playerId, [FromBody]NewItem item)
        {
            if(item.ItemType == Type.SWORD)
            {
                Player p = await repo.Get(playerId);
                if(p != null)
                {
                    if(p.Level < 3)
                    {
                        throw new NotAllowedException();
                    }
                }
            }
            
            Console.WriteLine("HttpPost: Create item");
            return await repo.CreateItem(playerId, item);
        }


        [HttpPost("{playerId:guid}/items/update")]
        public Task<Item> UpdateItem(Guid playerId, [FromBody]ModifiedItem item)
        {
            Console.WriteLine("HttpPost: UpdateItem");
            return repo.UpdateItem(playerId, item);
        }

        [HttpPost("{playerId:guid}/items/delete")]
        public Task<Item> DeleteItem(Guid playerId, [FromBody]Item item)
        {
            Console.WriteLine("HttpPost: DeleteItem");
            return repo.DeleteItem(playerId, item);   
        }

        [HttpOptions]
        public void Options() { }

    }
}