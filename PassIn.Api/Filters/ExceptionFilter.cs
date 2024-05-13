using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using System.Net;

namespace PassIn.Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var result = context.Exception is PassInException;
            if (result)
            {
                HandleProjectException(context);
            }
            else
            {
                ThrowUnknowError(context);
            }
        }

        private void HandleProjectException(ExceptionContext context)
        {
            if (context.Exception is NotFoundExcpetion)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Result = new NotFoundObjectResult(new ResponseErrorJson(context.Exception.Message));
            }
            else if (context.Exception is ErroOnValidationException)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Result = new BadRequestObjectResult(new ResponseErrorJson(context.Exception.Message));
            }
            else if (context.Exception is ConflictException)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Result = new ConflictObjectResult(new ResponseErrorJson(context.Exception.Message));
            }

        }

        private void ThrowUnknowError(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = ((int)HttpStatusCode.InternalServerError);
            context.Result = new ObjectResult(new ResponseErrorJson("Erro desconhecido"));
        }
    }

     
}
