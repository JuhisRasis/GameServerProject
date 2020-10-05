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

            Console.WriteLine("Do you want to play the game(0) or see the leaderboard(1)? (answer 0 or 1) \n");
            string answer = Console.ReadLine();

            if (answer == "0")
            {
                GuessNumberGame(player);
            }
        }

        void GuessNumberGame(Player myPlayer)
        {
            int ownNumber = 0;
            while (ownNumber == 0)
            {
                Console.WriteLine("Set your guessable number: (1-10) \n");
                ownNumber = Int32.Parse(Console.ReadLine());
                if (ownNumber > 10 || ownNumber < 1)
                {
                    Console.WriteLine("Number must be between 1 and 10! \n");
                    ownNumber = 0;
                }
            }
            myPlayer.GuessGameNumber = ownNumber;
            Task<Player> task = GetRandomPlayer();
            task.Wait();
            Console.WriteLine(task.Result.Name);

            //get paired with random player from player list
            //Player myPair;
        }
    }
}