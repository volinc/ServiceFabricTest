using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Common
{
    public abstract class ExceptionHandlerBase<THandler> where THandler : ExceptionHandlerBase<THandler>
    {        
        public const string UnhandledException = "Unhandled exception.";

        private readonly IHostingEnvironment env;

        protected ILogger<THandler> Logger { get; }

        protected ExceptionHandlerBase(IHostingEnvironment env, ILogger<THandler> logger)
        {
            this.env = env;

            Logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (exception == null) return;

            var result = Handle(exception);
            context.Response.StatusCode = (int)result.StatusCode;
            
            // см. https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reverseproxy
            if (result.StatusCode == HttpStatusCode.NotFound)
                context.Response.Headers.Add("X-ServiceFabric", "ResourceNotFound");

            var jsonApiErrors = new JsonApiErrors
            {
                Errors = new[]
                {
                    new JsonApiError
                    {                        
                        Title = result.Title,
                        Detail = env.IsDevelopment() ? exception.ToString() : null
                    }
                }
            };

            context.Response.ContentType = "application/json";

            using (var writer = new StreamWriter(context.Response.Body))
            {
                new JsonSerializer().Serialize(writer, jsonApiErrors);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }

        protected virtual ExceptionResult Handle(Exception exception)
        {
            switch (exception)
            {                   
                //case EntityForbiddenException _:
                //    return new ExceptionResult(HttpStatusCode.Forbidden, "Access to the resource is forbidden.");

                //case EntityNotFoundException _:
                //    return new ExceptionResult(HttpStatusCode.NotFound, "Entity not found.");

                //case EntityAlreadyExistsException _:
                //    return new ExceptionResult(HttpStatusCode.Conflict, "Entity already exists.");

                //case EntityException _:
                //    return new ExceptionResult(HttpStatusCode.BadRequest, exception.Message);

                //case DomainException _:
                //    return new ExceptionResult(HttpStatusCode.BadRequest, exception.Message);               

                default:
                    Logger.LogError(exception, UnhandledException);
                    return new ExceptionResult(HttpStatusCode.BadRequest, UnhandledException); 
            }
        }
    }
}
