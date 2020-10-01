using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;

namespace GameWebApi
{
    public class GameClass : MongoDbRepository
    {

        public void Game()
        {
            Console.WriteLine("asd");
            DoSomethingAsync();

        }
    }
}