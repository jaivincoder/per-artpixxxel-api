using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace api.artpixxel.Infrastructure.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            string message;

            switch (exception)
            {
                case ArgumentException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case KeyNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = "Resource not found";
                    break;

                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    message = "Unauthorized";
                    break;

                case SqlException:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    message = "Database error occurred";
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred";
                    break;
            }

            var result = JsonSerializer.Serialize(new
            {
                status = statusCode,
                success = false,
                error = message
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(result);
        }
    }

}
