using DockerUI.Api.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DockerUI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : Controller
    {
        public BaseApiController()
        {
        }

        protected ApiSuccessResponse<object> SuccessResponse(string message)
        {
            return new ApiSuccessResponse<object>(message);
        }
        protected ApiSuccessResponse<T> SuccessResponse<T>(T data, string message = "")
        {
            return new ApiSuccessResponse<T>(data, message);
        }

        protected ApiErrorResponse<object> ErrorResponse()
        {
            return new ApiErrorResponse<object>();
        }

        protected ApiErrorResponse<object> ErrorResponse(string message)
        {
            return new ApiErrorResponse<object>(message);
        }

        protected ApiErrorResponse<object> ErrorResponse(string message, string internalMessage)
        {
            return new ApiErrorResponse<object>(message)
            {
                InternalMessage = internalMessage
            };
        }

        protected ApiErrorResponse<T> ErrorResponse<T>(T data, string message = "")
        {
            return new ApiErrorResponse<T>(data, message);
        }

        protected ApiErrorResponse<T> ErrorResponse<T>(T data, List<Error> errors, string message = "")
        {
            return new ApiErrorResponse<T>(data, errors, message);
        }
    }
}
