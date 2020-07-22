using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DockerUI.Api.Services
{
    public static class ExceptionMiddleware
    {
        public static void UseExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
 
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if(contextFeature != null)
                    { 
                        var error = new DockerUI.Api.Dto.ApiErrorResponse<object>(contextFeature.Error.StackTrace,contextFeature.Error.Message);

                        var result = JsonConvert.SerializeObject(error);
                        await context.Response.WriteAsync(result);
                    }
                });
            });
        }
    }
}