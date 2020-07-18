using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DockerUI.Api.Dto
{
    public sealed class Error
    {
        public string Code { get; }
        public string Description { get; }
        public string Field { get; set; }
        public Error(string code, string description, string field = null)
        {
            Code = code;
            Description = description;
            Field = field;
        }
    }

    public abstract class ApiResponseBase<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string InternalMessage { get; set; }
        public T Data { get; set; }
        public List<Error> Errors { get; set; } = new List<Error>();
    }

    public class ApiSuccessResponse<T> : ApiResponseBase<T>
    {
        public ApiSuccessResponse()
        {
            Success = true;
        }
        public ApiSuccessResponse(string message)
            : this()
        {
            Message = message;
        }
        public ApiSuccessResponse(T data, string message = null)
            : this()
        {
            Message = message;
            Data = data;
        }

        public ObjectResult ToOk()
        {
            return new ObjectResult(this)
            {
                StatusCode = 200
            };
        }

        public ObjectResult ToCreated()
        {
            return new ObjectResult(this)
            {
                StatusCode = 201
            };
        }

        public ObjectResult ToAccepted()
        {
            return new ObjectResult(this)
            {
                StatusCode = 202
            };
        }

        public ObjectResult ToNoContent()
        {
            return new ObjectResult(this)
            {
                StatusCode = 202
            };
        }
    }

    public class ApiErrorResponse : ApiErrorResponse<object>
    {
        public ApiErrorResponse(string message)
            : base(message)
        {

        }
    }
    public class ApiErrorResponse<T> : ApiResponseBase<T>
    {
        public ApiErrorResponse()
        {
            Success = false;
        }

        public ApiErrorResponse(string message = null)
            : this()
        {
            Message = message;
        }

        public ApiErrorResponse(T data, string message = null)
            : this()
        {
            Message = message;
            Data = data;
        }

        public ApiErrorResponse(T data, List<Error> errors, string message = null)
            : this()
        {
            Message = message;
            Errors = errors;
            Message = message;
        }

        public ApiErrorResponse<T> ModelState(ModelStateDictionary modelState)
        {
            foreach (var state in modelState.Where(p => p.Value.ValidationState == ModelValidationState.Invalid).ToList())
            {
                foreach (var err in state.Value.Errors)
                {
                    Errors.Add(new Error("", err.ErrorMessage, state.Key));
                }
            }
            return this;
        }
        public ApiErrorResponse<T> Exception(Exception ex)
        {
            this.InternalMessage = ex.Message;

            if (ex.InnerException != null)
            {
                this.Errors.Add(new Error("", ex.InnerException.Message));
            }

            return this;
        }
        public ObjectResult ToBadRequest()
        {
            return new ObjectResult(this)
            {
                StatusCode = 400
            };
        }
        public ObjectResult ToUnauthorized()
        {
            return new ObjectResult(this)
            {
                StatusCode = 401
            };
        }
        public ObjectResult ToForbidden()
        {
            return new ObjectResult(this)
            {
                StatusCode = 403
            };
        }
        public ObjectResult ToNotfound()
        {
            return new ObjectResult(this)
            {
                StatusCode = 404
            };
        }
        public ObjectResult ToInternalServerError(Exception ex = null)
        {
            return new ObjectResult(this)
            {
                StatusCode = 500
            };
        }
        public ObjectResult ToServiceUnavailable()
        {
            return new ObjectResult(this)
            {
                StatusCode = 503
            };
        }
        public ObjectResult ToRedirect(string url)
        {
            return new ObjectResult(this)
            {
                StatusCode = (int)HttpStatusCode.Redirect,
                Value = url
            };
        }
    }
}
