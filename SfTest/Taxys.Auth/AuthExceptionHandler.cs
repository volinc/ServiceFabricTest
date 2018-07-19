using Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Taxys.Auth
{
    public class AuthExceptionHandler : ExceptionHandlerBase<AuthExceptionHandler>
    {
        public AuthExceptionHandler(IHostingEnvironment env, ILogger<AuthExceptionHandler> logger) : base(env, logger)
        {
        }
    }
}
