using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WebApi.Entities.Auth;
using WebApi.Helpers.Auth;
using WebApi.Models.Auth;
using WebApi.Services.Auth;

namespace WebApi.Controllers.Auth
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountsController(
            IAccountService accountService)
        {
            _accountService = accountService;
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
                return Problem(e.Message, null, e.StatusCode, null, e.Type);
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
                    return BadRequest(new { message = "Token is required" });

                // users can revoke their own tokens and admins can revoke any tokens
                if (!Account.OwnsToken(token) && Account.Role != Role.Admin)
                    return Unauthorized(new { message = "Unauthorized" });

                _accountService.RevokeToken(token, IpAddress());
                return Ok(new { message = "Token revoked" });
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
            return Ok(new { message = "Registration successful, please check your email for verification instructions" });
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
                return Ok(new { message = "Verification successful, you can now login" });
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
                return Ok(new { message = "Please check your email for password reset instructions" });
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
                return Ok(new { message = "Token is valid" });
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
                return Ok(new { message = "Password reset successful, you can now login" });
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, null, e.Type);
            }
        }

        [Authorize(Role.Admin)]
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
        [HttpGet("{id:int}")]
        public ActionResult<AccountResponse> GetById(int id)
        {
            // users can get their own account and admins can get any account
            if (id != Account.Id && Account.Role != Role.Admin)
                return Unauthorized(new { message = "Unauthorized" });

            var account = _accountService.GetById(id);
            return Ok(account);
        }

        [Authorize(Role.Admin)]
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
        [HttpPut("{id:int}")]
        public ActionResult<AccountResponse> Update(int id, UpdateRequest model)
        {
            try
            {
                // users can update their own account and admins can update any account
                if (id != Account.Id && Account.Role != Role.Admin)
                {
                    //return Unauthorized(new { message = "Unauthorized" });
                    throw new AppException("You are not allowed to update others account.", 401, "Unauthorized");
                }

                // only admins can update role
                if (Account.Role != Role.Admin)
                    model.Role = null;

                var account = _accountService.Update(id, model);
                return Ok(account);
            }
            catch (AppException e)
            {
                return Problem(e.Message, null, e.StatusCode, null, e.Type);
            }
            catch (Exception e)
            {
                return Problem(e.Message, null, 400, null, "BadRequest");
            }
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                // users can delete their own account and admins can delete any account
                if (id != Account.Id && Account.Role != Role.Admin)
                    return Unauthorized(new { message = "Unauthorized" });

                _accountService.Delete(id);
                return Ok(new { message = "Account deleted successfully" });
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
}
