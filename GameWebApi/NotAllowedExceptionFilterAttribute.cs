using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GameWebApi
{
    public class NotAllowedExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if(context.Exception is NotAllowedException)
            {
            Console.WriteLine("Sword cannot be given to a player below level 3");
               context.Result = new ContentResult
               {
                   Content = "Error: Sword cannot be given to a player below level 3",
                   ContentType = "text/plain",
                   StatusCode = 405
               };
            }
        }
    }
}