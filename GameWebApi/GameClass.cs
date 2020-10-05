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

        Player opponent;

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

                    Console.WriteLine("Your current rating is: " + x.Score);
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


            bool foundPlayer = false;
            while (foundPlayer == false)
            {
                Task<Player> task = GetRandomPlayer();
                task.Wait();
                Console.WriteLine(task.Result.Name);
                if (task.Result.Name != myPlayer.Name)
                {
                    foundPlayer = true;
                    opponent = task.Result;
                }

            }
            Console.WriteLine("Your Opponent is: " + opponent.Name + "\n");
            Console.WriteLine("Guess " + opponent.Name + "'s number!");

            int guessNumberInt = 0;
            while (guessNumberInt == 0)
            {
                guessNumberInt = Int32.Parse(Console.ReadLine());
                if (guessNumberInt > 10 || guessNumberInt < 1)
                {
                    Console.WriteLine("Number must be between 1 and 10! \n");
                    guessNumberInt = 0;
                }
            }
            Task<Player> getPlayerTask = GetPlayerWithName(opponent.Name);
            getPlayerTask.Wait();
            if (getPlayerTask.Result.GuessGameNumber == guessNumberInt)
            {
                Console.WriteLine("congratulations you win +10 score, " + opponent.Name + "lost -10 score\n");
                Task<Player> updateScoreTask = UpdatePlayerGuessNameNumber(opponent.Name, opponent.Score - 10);
                updateScoreTask.Wait();
                Task<Player> updateMyScoreTask = UpdatePlayerGuessNameNumber(myPlayer.Name, myPlayer.Score + 10);
                updateMyScoreTask.Wait();
                Console.WriteLine("Your current rating is: " + myPlayer.Score + "" + (opponent.Score - 10));
            }
            else
            {
                Console.WriteLine("You lost -1 score, " + opponent.Name + " won +1 score\n");
                Task<Player> updateScoreTask = UpdatePlayerGuessNameNumber(opponent.Name, opponent.Score + 1);
                updateScoreTask.Wait();
                Task<Player> updateMyScoreTask = UpdatePlayerGuessNameNumber(myPlayer.Name, myPlayer.Score - 1);
                updateMyScoreTask.Wait();
                Console.WriteLine("Your current rating is: " + myPlayer.Score + " " + opponent.Name + "'s current rating is: " + (opponent.Score + 1));
            }

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
                startPosInt = Int32.Parse(startPos);

            } while (startPosInt > amount);


            Console.WriteLine("How many players do you want to display? ");
            int howManyInt = 0;

            do
            {
                Console.WriteLine("How many positions do you want to display? Max Amount:  " + (amount - startPosInt));
                string howMany = Console.ReadLine();
                howManyInt = Int32.Parse(howMany);

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