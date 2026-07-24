using System.Net;
using Core.Requests;
using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace WebApi
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) => _logger = logger;

        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);

            ErrorResponseRequest response = new ErrorResponseRequest
            {
                Message = context.Exception.Message
            };

            BaseException? baseException = context.Exception as BaseException;
            CarEnumNotDefinedException? carEnumNotDefinedException =
                context.Exception as CarEnumNotDefinedException;
            CarNotFoundException? carNotFoundException = context.Exception as CarNotFoundException;
            CouldntSendConfirmEmail? couldntSendConfirmEmail =
                context.Exception as CouldntSendConfirmEmail;
            DoesNotManageException? doesNotManageException =
                context.Exception as DoesNotManageException;
            EmailNotConfirmedException? emailNotConfirmedException =
                context.Exception as EmailNotConfirmedException;
            EmptyInputException? emptyInputException = context.Exception as EmptyInputException;
            IncorrectUserNameOrPasswordException? incorrectUserNameOrPasswordException =
                context.Exception as IncorrectUserNameOrPasswordException;
            KeyAlreadyExistsException? keyAlreadyExistsException =
                context.Exception as KeyAlreadyExistsException;
            PasswordsDontMatchException? passwordsDontMatchException =
                context.Exception as PasswordsDontMatchException;
            ProfileException? profileException = context.Exception as ProfileException;
            RoleDoesntExistException? roleDoesntExistException =
                context.Exception as RoleDoesntExistException;
            RoleExistsException? roleExistsException = context.Exception as RoleExistsException;
            UserNotFoundException? userNotFoundException =
                context.Exception as UserNotFoundException;
            UserNotInRoleException? userNotInRoleException =
                context.Exception as UserNotInRoleException;

            if (carEnumNotDefinedException != null)
            {
                response.Message = carEnumNotDefinedException.Message;
                response.StatusCode = carEnumNotDefinedException.StatusCode;
            }
            else if (carNotFoundException != null)
            {
                response.Message = carNotFoundException.Message;
                response.StatusCode = carNotFoundException.StatusCode;
            }
            else if (couldntSendConfirmEmail != null)
            {
                response.Message = couldntSendConfirmEmail.Message;
                response.StatusCode = couldntSendConfirmEmail.StatusCode;
            }
            else if (doesNotManageException != null)
            {
                response.Message = doesNotManageException.Message;
                response.StatusCode = doesNotManageException.StatusCode;
            }
            else if (emailNotConfirmedException != null)
            {
                response.Message = emailNotConfirmedException.Message;
                response.StatusCode = emailNotConfirmedException.StatusCode;
            }
            else if (emptyInputException != null)
            {
                response.Message = emptyInputException.Message;
                response.StatusCode = emptyInputException.StatusCode;
            }
            else if (incorrectUserNameOrPasswordException != null)
            {
                response.Message = incorrectUserNameOrPasswordException.Message;
                response.StatusCode = incorrectUserNameOrPasswordException.StatusCode;
            }
            else if (keyAlreadyExistsException != null)
            {
                response.Message = keyAlreadyExistsException.Message;
                response.StatusCode = keyAlreadyExistsException.StatusCode;
            }
            else if (passwordsDontMatchException != null)
            {
                response.Message = passwordsDontMatchException.Message;
                response.StatusCode = passwordsDontMatchException.StatusCode;
            }
            else if (profileException != null)
            {
                response.Message = profileException.Message;
                response.StatusCode = profileException.StatusCode;
            }
            else if (roleDoesntExistException != null)
            {
                response.Message = roleDoesntExistException.Message;
                response.StatusCode = roleDoesntExistException.StatusCode;
            }
            else if (roleExistsException != null)
            {
                response.Message = roleExistsException.Message;
                response.StatusCode = roleExistsException.StatusCode;
            }
            else if (userNotFoundException != null)
            {
                response.Message = userNotFoundException.Message;
                response.StatusCode = userNotFoundException.StatusCode;
            }
            else if (userNotInRoleException != null)
            {
                response.Message = userNotInRoleException.Message;
                response.StatusCode = userNotInRoleException.StatusCode;
            }
            else
            {
                response.Message =
                    $"Oops. Something went wrong!{Environment.NewLine}{context.Exception.Message}";
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            var json = JsonConvert.SerializeObject(response);

            context.Result = new JsonResult(json)
            {
                StatusCode = (int?)baseException?.StatusCode ?? 500
            };
        }
    }
}
