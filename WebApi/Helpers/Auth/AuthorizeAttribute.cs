using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Entities.Auth;

namespace WebApi.Helpers.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IList<Role> _roles;

        public AuthorizeAttribute(params Role[] roles)
        {
            _roles = roles ?? Array.Empty<Role>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Type localizerType = GetLocalizerType(context);
            IStringLocalizer localizer = (IStringLocalizer)context.HttpContext.RequestServices.GetService(localizerType);
            //
            var account = (Account)context.HttpContext.Items["Account"];
            if (account == null || _roles.Any() && !_roles.Contains(account.Role))
            {
                // not logged in or role not authorized
                context.Result = new JsonResult(new MessageRecord(localizer["Unauthorized"].Value))
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }

        private static Type GetLocalizerType(AuthorizationFilterContext context)
        {
            var controllerType = (context.ActionDescriptor as ControllerActionDescriptor).ControllerTypeInfo;
            return typeof(IStringLocalizer<>).MakeGenericType(controllerType);
        }
    }

    internal record MessageRecord(string Message);
}