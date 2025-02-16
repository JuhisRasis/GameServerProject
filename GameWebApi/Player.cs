using System;
using System.Collections.Generic;

namespace GameWebApi
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public bool IsBanned { get; set; }
        public DateTime CreationTime { get; set; }
        public int GuessGameNumber { get; set; }
    }
}
