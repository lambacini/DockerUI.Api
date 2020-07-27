using System;
using System.Net;
using Docker.DotNet;
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
                    if (contextFeature != null)
                    {
                        DockerUI.Api.Dto.ApiResponseBase<object> error = null;
                        if (contextFeature.Error is DockerApiException)
                        {
                            var dockerException = contextFeature.Error as DockerApiException;
                            error = new DockerUI.Api.Dto.ApiErrorResponse<object>(dockerException, dockerException.ResponseBody);
                        }
                        else
                        {
                            error = new DockerUI.Api.Dto.ApiErrorResponse<object>(contextFeature.Error, contextFeature.Error.Message);
                        }


                        var result = JsonConvert.SerializeObject(error);
                        await context.Response.WriteAsync(result);
                    }
                });
            });
        }
    }
}