using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entities.Auth;

namespace WebApi.Controllers.Auth
{
    // a record for creating error messages
    internal record MessageRecord(string Message);

    [Controller]
    public abstract class BaseController : ControllerBase
    {
        // returns the current authenticated account from the http payload (null if not logged in)
        public Account Account => (Account)HttpContext.Items["Account"];
    }
}
