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
        Player x;


        public void Game()
        {
            Console.WriteLine("Welcome to Gaem\n");

            Console.WriteLine("Give Player Name\n");
            string playerName = Console.ReadLine();


            Player newPlayer = new Player();
            newPlayer.Name = playerName;
            Console.WriteLine("Your character name is " + newPlayer.Name + "\n");


            Task<Player> task = GetPlayerName(newPlayer);
            Console.WriteLine("Please wait patiently " + "while I fetch player Information");
            try
            {

                task.Wait();
                x = task.Result;
                if (x == newPlayer)
                {
                    Console.WriteLine("Welcome Back " + x.Name);
                }
            }
            catch
            {

                Console.WriteLine("New Account Created");

            }



            AddPlayerInformation(newPlayer);


        }
        public async Task<Player> GetPlayerName(Player playerName)
        {
            await GetPlayerWithName(playerName.Name);
            return playerName;

        }
    }


}