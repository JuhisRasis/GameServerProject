using System.Collections.Generic;
namespace GameWebApi
{
    public class NewPlayer
    {
        public string Name { get; set; }
        public int Level { get; set; } // Set this in attribute in Json body to test sword restriction(otherwise defaults to 0?)
        public List<string> Tags { get; set; }
    }
}
