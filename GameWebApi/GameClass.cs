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
            Console.WriteLine("Welcome to Gaem\n");

            Console.WriteLine("Give Player Name\n");
            string playerName = Console.ReadLine();


            Player newPlayer = new Player();
            newPlayer.Name = playerName;
            Console.WriteLine("Your character name is " + newPlayer.Name + "\n");






            AddPlayerInformation(newPlayer);

        }
        public async Task<Player> GetPlayerName(Player playerName)
        {

            await GetPlayerWithName(playerName.Name);
            return playerName;

        }
    }


}