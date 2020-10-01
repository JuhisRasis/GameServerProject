using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GameWebApi
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(NotFoundException e)
            {
                context.Response.HttpContext.Response.StatusCode = 404;
            }
        }
    }
}