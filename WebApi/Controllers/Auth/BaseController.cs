using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entities.Auth;

namespace WebApi.Controllers.Auth
{
    [Controller]
    public abstract class BaseController : ControllerBase
    {
        // returns the current authenticated account (null if not logged in)
        public Account Account => (Account)HttpContext.Items["Account"];
    }
}
