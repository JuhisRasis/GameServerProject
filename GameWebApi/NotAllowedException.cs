using System;


namespace GameWebApi
{
    public class NotAllowedException : Exception
    {
       public NotAllowedException()
       {
           Console.WriteLine("NotAllowedException thrown");
       }
    }
}