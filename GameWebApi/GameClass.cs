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
                Leaderboard(player);
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
            Task<Player> getMyPlayerTask = GetPlayerWithName(myPlayer.Name);
            getMyPlayerTask.Wait();
            myPlayer = getMyPlayerTask.Result;

            if (getPlayerTask.Result.GuessGameNumber == guessNumberInt)
            {
                Console.WriteLine("Congratulations you win +10 score, " + opponent.Name + " lost -10 score\n");
                Task<Player> updateScoreTask = UpdatePlayerScore(opponent.Name, (opponent.Score - 10));
                updateScoreTask.Wait();

                Task<Player> updateMyScoreTask = UpdatePlayerScore(myPlayer.Name, (myPlayer.Score + 10));
                updateMyScoreTask.Wait();
                Console.WriteLine("Your current rating is: " + (myPlayer.Score + 10) + " opponent's current rating is: " + (opponent.Score - 10));
            }
            else
            {
                Console.WriteLine("You lost -1 score, " + opponent.Name + " won +1 score\n");
                Task<Player> updateScoreTask = UpdatePlayerScore(opponent.Name, (opponent.Score + 1));
                updateScoreTask.Wait();

                Task<Player> updateMyScoreTask = UpdatePlayerScore(myPlayer.Name, (myPlayer.Score - 1));
                updateMyScoreTask.Wait();
                Console.WriteLine("Your current rating is: " + (myPlayer.Score - 1) + " " + opponent.Name + "'s current rating is: " + (opponent.Score + 1));
            }

            //get paired with random player from player list
            //Player myPair;
        }
        void Leaderboard(Player player)
        {
            Console.WriteLine("Do you want to search leaderboard postions(0) or see your current position in the leaderboard(1)\n or Cycle trough leaderboard 10 pages at a time(2) or display top10 Players(3)? (answer 0 or 1 or 2 or 3) \n");
            string answer = Console.ReadLine();

            if (answer == "0")
            {
                SearchPositions();
            }
            else if (answer == "1")
            {
                DisplayMyPostion(player);
            }
            else if (answer == "2")
            {
                CycleThroughLeaderboard();
            }
            else
            {
                DisplayTop10Players();
            }
        }

        void SearchPositions()
        {
            Task<long> documentTask = GetDocumentAmount();
            documentTask.Wait();
            amount = Convert.ToInt32(documentTask.Result);


            int startPosInt = 0;
            do
            {
                Console.WriteLine("Give leaderboard search position? (Descending order) First position: 0 -  last position:  " + amount);
                string startPos = Console.ReadLine();
                startPosInt = Int32.Parse(startPos);

            } while (startPosInt > amount);


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
                Console.WriteLine("Pos. " + (startPosInt + i + 1) + "       Name: " + xArray[i].Name + "      Score: " + xArray[i].Score + "\n");
            }
        }

        void DisplayMyPostion(Player player)
        {
            Task<long> documentTask = GetDocumentAmount();
            documentTask.Wait();
            amount = Convert.ToInt32(documentTask.Result);

            Task<Player[]> task = GetMyPlayerRanking(amount, player);
            Console.WriteLine("Please wait patiently " + "while I fetch leaderboard");
            task.Wait();
            xArray = task.Result;
            for (int i = 0; i < amount; i++)
            {
                if (xArray[i].Name == player.Name)
                {
                    Console.WriteLine("Pos. " + (i + 1) + "       Name: " + xArray[i].Name + "      Score: " + xArray[i].Score + "\n");
                }
            }
        }

        void CycleThroughLeaderboard()
        {
            Console.WriteLine("Please wait patiently " + "while I fetch leaderboard");
            int currentPage = 0;
            int i = 0;
            Task<long> documentTask = GetDocumentAmount();
            documentTask.Wait();
            amount = Convert.ToInt32(documentTask.Result);

            while (i <= amount)
            {

                Task<Player[]> task = Get10PlayersAtAtime(currentPage, 10);
                task.Wait();
                xArray = task.Result;

                for (int i2 = 0; i2 < 10; i2++)
                {
                    Console.WriteLine("Pos. " + (i + 1) + "       Name: " + xArray[i2].Name + "      Score: " + xArray[i2].Score + "\n");
                    i++;
                }
                Console.WriteLine("Display next 10 positions? Press Enter");
                Console.ReadLine();
                currentPage++;
                Console.WriteLine("Current page is: " + currentPage);
            }
            Console.WriteLine("EndLoop");
        }


        void DisplayTop10Players()
        {
            Console.WriteLine("Please wait patiently " + "while I fetch leaderboard");
            Task<Player[]> task = GetTop10Players();
            task.Wait();
            xArray = task.Result;
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Pos. " + (i + 1) + "       Name: " + xArray[i].Name + "      Score: " + xArray[i].Score + "\n");
            }
        }
    }
}