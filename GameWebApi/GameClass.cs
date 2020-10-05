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

            Player player = new Player();
            player.Name = playerName;
            Console.WriteLine("Your character name is " + player.Name + "\n");

            try
            {
                Task<Player> task = GetPlayerWithName(playerName);
                Console.WriteLine("Please wait patiently " + "while I fetch player Information");
                task.Wait();
                Console.WriteLine(task.Result);
                x = task.Result;
                if (x.Name == player.Name)
                {
                    Console.WriteLine("Welcome Back " + x.Name);
                    Console.WriteLine(x.CreationTime);
                    Console.WriteLine(x.IsBanned);
                    Console.WriteLine(x.Score);
                }
            }
            catch
            {

                Console.WriteLine("New Account Created");
                AddPlayerInformation(player);
            }
        }
    }
}