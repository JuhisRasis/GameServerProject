using System;

namespace GameWebApi
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
            Console.WriteLine("NotFoundException thrown");
        }
    }
}