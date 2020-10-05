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
        Player[] xArray;
        int amount;


        public void Game()
        {
            Console.WriteLine("Welcome to Gaem\n");

            Console.WriteLine("Give Player Name\n");
            string playerName = Console.ReadLine();

            Player player = new Player();
            player.Name = playerName;
            Console.WriteLine("Your character name is " + player.Name + "\n");

            Console.WriteLine("Please wait patiently " + "while I fetch player Information");

            try
            {
                Task<Player> task = GetPlayerWithName(playerName);
                task.Wait();
                x = task.Result;
                if (task.IsCompleted)
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
            else
            {
                Leaderboard();
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

            Task<Player> updateTask = UpdatePlayerGuessNameNumber(myPlayer.Name, ownNumber);
            updateTask.Wait();

            Task<Player> task = GetRandomPlayer();
            task.Wait();
            Console.WriteLine(task.Result.Name);

            //get paired with random player from player list
            //Player myPair;
        }
        void Leaderboard()
        {
            Task<long> documentTask = GetDocumentAmount();
            documentTask.Wait();
            amount = Convert.ToInt32(documentTask.Result);
            int startPosInt = 0;
            do
            {
                Console.WriteLine("Give leaderboard search position? last pos:  " + amount);
                string startPos = Console.ReadLine();
                startPosInt = Int16.Parse(startPos);

            } while (startPosInt > amount);


            Console.WriteLine("How many players do you want to display? ");
            int howManyInt = 0;

            do
            {
                Console.WriteLine("How many positions do you want to display? Max Amount:  " + (amount - startPosInt));
                string howMany = Console.ReadLine();
                howManyInt = Int16.Parse(howMany);

            } while (startPosInt > amount);

            //Console.WriteLine("My position is " + );

            Task<Player[]> task = GetlayersWithinThesePostions(startPosInt, howManyInt);
            Console.WriteLine("Please wait patiently " + "while I fetch leaderboard");
            task.Wait();
            xArray = task.Result;
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Name: " + xArray[i].Name + "      Score: " + xArray[i].Score + "\n");

            }
        }
    }
}