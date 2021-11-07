using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using WebApi.Entities.Auth;
using WebApi.Helpers.Auth;
using WebApi.Models.Auth;
using WebApi.Services.Auth;

namespace WebApi.Controllers.Auth
{
    [ApiController]
    [Route("{culture:culture}/[controller]")]
    [Produces("application/json")]
    public class AccountsController : BaseController
    {
        private readonly IStringLocalizer<AccountsController> _localizer;
        private readonly IAccountService _accountService;

        public AccountsController(
            IAccountService accountService,
            IStringLocalizer<AccountsController> localizer)
        {
            _accountService = accountService;
            _localizer = localizer;
        }

        [HttpPost("authenticate")]
        public ActionResult<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {            
            try
            {
                var response = _accountService.Authenticate(model, IpAddress());
                SetTokenCookie(response.RefreshToken);
                return Ok(response);
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, null, e.Type);
            }
        }

        [HttpPost("refresh-token")]
        public ActionResult<AuthenticateResponse> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                var response = _accountService.RefreshToken(refreshToken, IpAddress());
                SetTokenCookie(response.RefreshToken);
                return Ok(response);
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, e.Title, e.Type);
            }
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public IActionResult RevokeToken(RevokeTokenRequest model)
        {
            try
            {
                // accept token from request body or cookie
                var token = model.Token ?? Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(token))
                    return BadRequest(new MessageRecord(_localizer["TokenRequired"].Value));

                // users can revoke their own tokens and admins can revoke any tokens
                if (!Account.OwnsToken(token) && Account.Role != Role.Super)
                    return Unauthorized(new MessageRecord(_localizer["Unauthorized"].Value));

                _accountService.RevokeToken(token, IpAddress());
                return Ok(new MessageRecord(_localizer["TokenRevoked"].Value));
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, null, e.Type);
            }
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest model)
        {
            try { 
            _accountService.Register(model, Request.Headers["origin"]);
            return Ok(new MessageRecord(_localizer["RegistrationSuccessful"].Value));
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, null, e.Type);
            }
        }

        [HttpPost("verify-email")]
        public IActionResult VerifyEmail(VerifyEmailRequest model)
        {
            try
            {
                _accountService.VerifyEmail(model.Token);
                return Ok(new MessageRecord(_localizer["VerificationSuccessful"].Value));
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, null, e.Type);
            }
        }

        [HttpPost("resend-verification-token")]
        public IActionResult ResendVerificationToken(ResendVerificationTokenRequest model)
        {
            try
            {
                _accountService.ResendVerificationToken(model, Request.Headers["origin"]);
                return Ok(new MessageRecord(_localizer["CheckEmailVerification"].Value));
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, null, e.Type);
            }
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(ForgotPasswordRequest model)
        {
            try
            {
                _accountService.ForgotPassword(model, Request.Headers["origin"]);
                return Ok(new MessageRecord(_localizer["CheckEmailPasswordReset"].Value));
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, null, e.Type);
            }
        }

        [HttpPost("validate-reset-token")]
        public IActionResult ValidateResetToken(ValidateResetTokenRequest model)
        {
            try
            {
                _accountService.ValidateResetToken(model);
                return Ok(new MessageRecord(_localizer["TokenValid"].Value));
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, null, e.Type);
            }
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordRequest model)
        {
            try
            {
                _accountService.ResetPassword(model);
                return Ok(new MessageRecord(_localizer["PasswordResetSuccessful"].Value));
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, null, e.Type);
            }
        }

        [Authorize(Role.Super)]
        [HttpGet]
        public ActionResult<IEnumerable<AccountResponse>> GetAll()
        {
            try
            {
                var accounts = _accountService.GetAll();
                return Ok(accounts);
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, null, e.Type);
            }
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public ActionResult<AccountResponse> GetById(Guid id)
        {
            // users can get their own account and admins can get any account
            if (id != Account.Id && Account.Role != Role.Super)
                return Unauthorized(new MessageRecord(_localizer["Unauthorized"].Value));

            var account = _accountService.GetById(id);
            return Ok(account);
        }

        [Authorize(Role.Super)]
        [HttpPost]
        public ActionResult<AccountResponse> Create(CreateRequest model)
        {
            try
            {
                var account = _accountService.Create(model);
                return Ok(account);
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, null, e.Type);
            }
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        public ActionResult<AccountResponse> Update(Guid id, UpdateRequest model)
        {
            try
            {
                // users can update their own account and admins can update any account
                if (id != Account.Id && Account.Role != Role.Super)
                {
                    //return Unauthorized(new { message = "Unauthorized" });
                    throw new AppException(_localizer["OthersAccountUpdate"].Value, 
                                            401, 
                                            "Unauthorized",
                                            _localizer["Unauthorized"].Value);
                }

                // only admins can update role
                if (Account.Role != Role.Super)
                    model.Role = null;

                var account = _accountService.Update(id, model);
                return Ok(account);
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, e.Title, e.Type);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 400, _localizer["BadRequest"].Value, "BadRequest");
            }
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                // users can delete their own account and admins can delete any account
                if (id != Account.Id && Account.Role != Role.Super)
                    return Unauthorized(new MessageRecord(_localizer["Unauthorized"].Value));

                _accountService.Delete(id);
                return Ok(new MessageRecord(_localizer["AccountDeleteSuccessful"].Value));
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, null, e.Type);
            }
        }

        // helper methods
        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                // Set the secure flag, which Chrome's changes will require for SameSite none.
                // Note this will also require you to be running on HTTPS.
                Secure = true,

                // Set the cookie to HTTP only which is good practice unless you really do need
                // to access it client side in scripts.
                HttpOnly = false,

                // Add the SameSite attribute, this will emit the attribute with a value of none.
                // To not emit the attribute at all set SameSite as (SameSiteMode)(-1)
                SameSite = SameSiteMode.None,

                // Expires time
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }

    internal record MessageRecord(string Message);
}
