using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameWebApi
{
    public class FileRepository// : IRepository
    {
        string path = @System.IO.Directory.GetCurrentDirectory() + "\\game-dev.txt";

        public async Task<Player> Get(Guid id)
        {
            
            Player player = new Player();
            player.Id = id;
            bool playerFound = false;

            using (StreamReader sr = File.OpenText(path))
            {
                string s = "";

                while ((s = sr.ReadLine()) != null)
                {
                    if (s == "    " + "Id: " + id)
                    {
                        //Console.WriteLine("Player found");
                        playerFound = true;
                        break;
                    }
                }

                if (playerFound)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        if ((s = sr.ReadLine()) != null)
                        {
                            //Console.WriteLine("Line:" + s);
                            switch (i)
                            {
                                case 0:
                                    player.Name = s.Substring(10, s.Length - 10);
                                    //Console.WriteLine("Name: " + player.Name);
                                    break;
                                case 1:
                                    int score = 0;
                                    int.TryParse(s.Substring(11, s.Length - 11), out score);
                                    player.Score = score;
                                    //Console.WriteLine("Score: " + player.Score);
                                    break;
                                case 2:
                                    int level = 0;
                                    int.TryParse(s.Substring(11, s.Length - 11), out level);
                                    player.Level = level;
                                    //Console.WriteLine("Level: " + player.Level);
                                    break;
                                case 3:
                                    bool IsBanned = false;
                                    IsBanned = bool.Parse(s.Substring(14, s.Length - 14));
                                    player.IsBanned = IsBanned;
                                    //Console.WriteLine("IsBanned: " + player.IsBanned);
                                    break;
                                case 4:
                                    DateTime date;
                                    date = DateTime.Parse(s.Substring(18, s.Length - 18));
                                    player.CreationTime = date;
                                    //Console.WriteLine("Date: " + player.CreationTime);
                                    break;
                            }
                        }
                    }
                }
            }
            if (!playerFound)
            {
                Console.WriteLine("Player with id: " +  id + " was not found!");
                return null;
            }
            return player;
        }
        public async Task<Player[]> GetAll()
        {
            Player[] players;
            List<Player> playersList = new List<Player>();
            List<Guid> ids = new List<Guid>();

            using (StreamReader sr = File.OpenText(path))
            {
                string s = "";

                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Contains("Id: "))
                    {
                        if (s.Substring(0, 8) == "    " + "Id: ")
                        {
                            if (s.Substring(8).Length == 36)
                            {
                                //Console.WriteLine("Id: " + s.Substring(8));
                                ids.Add(new Guid(s.Substring(8)));
                            }
                        }
                    }
                }
            }

            foreach (Guid id in ids)
            {
                Player p = await Get(id);
                playersList.Add(p);
            }

            players = playersList.ToArray();

            if (players != null)
            {
                return players;
            }
            else
            {
                Console.WriteLine("No players found!");
                return null;
            }
        }
        public async Task<Player> Create(Player player)
        {
            Console.WriteLine("Writing player object to a file. Id: " + player.Id);

            List<string> linesToAppend = new List<string>();

            linesToAppend.Add("Player:");
            linesToAppend.Add("    Id: " + player.Id);
            linesToAppend.Add("    Name: " + player.Name);
            linesToAppend.Add("    Score: " + player.Score);
            linesToAppend.Add("    Level: " + player.Level);
            linesToAppend.Add("    IsBanned: " + player.IsBanned.ToString());
            linesToAppend.Add("    CreationTime: " + player.CreationTime);

            //Console.WriteLine("File found");
            await File.AppendAllLinesAsync(path, linesToAppend.ToArray());

            Console.WriteLine("Successfully created player with Id: " + player.Id);

            return player;
        }
        public async Task<Player> Modify(Guid id, ModifiedPlayer player)
        {

            int line = 0;
            bool playerFound = false;

            using (StreamReader sr = File.OpenText(path))
            {
                string s = "";

                while ((s = await sr.ReadLineAsync()) != null)
                {
                    if (s == "    " + "Id: " + id)
                    {
                        //Console.WriteLine("Player found");
                        playerFound = true;
                        break;
                    }
                    line++;
                }
            }

            if(playerFound)
            {
                string[] lines = await File.ReadAllLinesAsync(path);
                lines[line + 2] = "    Score: " + player.Score;
                await File.WriteAllLinesAsync(path, lines);
            }
            
            Player p = await Get(id);

            if (p != null)
            {
                Console.WriteLine("Modified player: " + id + " successfully");
                return p;
            }
            else
            {
                Console.WriteLine("Couldnt modify player: " + id);
                return null;
            }
        }
        public async Task<Player> Delete(Guid id)
        {
            int line = 0;
            bool playerFound = false;

            using (StreamReader sr = File.OpenText(path))
            {
                string s = "";

                while ((s = await sr.ReadLineAsync()) != null)
                {
                    if (s == "    " + "Id: " + id)
                    {
                        playerFound = true;
                        //Console.WriteLine("Player found");
                        break;
                    }
                    line++;
                }
            }

            Player p = new Player();
            
            if (playerFound)
            {
                p = await Get(id); // Save soon to be deleted player to a variable
                string[] lines = await File.ReadAllLinesAsync(path);
                List<int> linesToRemove = new List<int> { line - 1, line, line + 1, line + 2, line + 3, line + 4, line + 5 };
                List<string> outputLines = new List<string>();

                for (int i = 0; i < lines.Length; i++)
                {
                    if (!linesToRemove.Contains(i))
                    {
                        outputLines.Add(lines[i]);
                    }
                }

                await File.WriteAllLinesAsync(path, outputLines.ToArray());
                Console.WriteLine("Player with id: " + id + " deleted successfully");
                // Return deleted player object
                return p;
            }
            else
            {
                Console.WriteLine("Couldnt find player to delete: " + id);
                return null;
            }
        }
    }
}