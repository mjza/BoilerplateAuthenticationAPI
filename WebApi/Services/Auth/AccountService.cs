using AutoMapper;
using BC = BCrypt.Net.BCrypt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Entities.Auth;
using WebApi.Helpers.Auth;
using WebApi.Models.Auth;

namespace WebApi.Services.Auth
{
    public interface IAccountService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        void RevokeToken(string token, string ipAddress);
        void Register(RegisterRequest model, string origin);
        void VerifyEmail(string token);
        void ForgotPassword(ForgotPasswordRequest model, string origin);
        void ResendVerificationToken(ResendVerificationTokenRequest model, string origin);
        void ValidateResetToken(ValidateResetTokenRequest model);
        void ResetPassword(ResetPasswordRequest model);
        IEnumerable<AccountResponse> GetAll();
        AccountResponse GetById(Guid id);
        AccountResponse Create(CreateRequest model);
        AccountResponse Update(Guid id, UpdateRequest model);
        void Delete(Guid id);
    }

    public class AccountService : IAccountService
    {
        private readonly AccountDbContext _accountDbContext;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;

        public AccountService(
            AccountDbContext accountDbContext,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IEmailService emailService)
        {
            _accountDbContext = accountDbContext;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailService = emailService;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var account = _accountDbContext.Accounts.SingleOrDefault(x => x.Email == model.Email);

            if (account == null || !account.IsVerified || !BC.Verify(model.Password, account.PasswordHash))
                throw new AppException("Email or password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = GenerateJwtToken(account);
            var refreshToken = GenerateRefreshToken(ipAddress);
            account.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from account
            RemoveOldRefreshTokens(account);

            // save changes to db
            _accountDbContext.Update(account);
            _accountDbContext.SaveChanges();

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = refreshToken.Token;
            return response;
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var (refreshToken, account) = GetRefreshToken(token);

            // replace old refresh token with a new one and save
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            account.RefreshTokens.Add(newRefreshToken);

            RemoveOldRefreshTokens(account);

            _accountDbContext.Update(account);
            _accountDbContext.SaveChanges();

            // generate new jwt
            var jwtToken = GenerateJwtToken(account);

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = newRefreshToken.Token;
            return response;
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var (refreshToken, account) = GetRefreshToken(token);

            // revoke token and save
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _accountDbContext.Update(account);
            _accountDbContext.SaveChanges();
        }

        public void Register(RegisterRequest model, string origin)
        {
            // validate
            if (_accountDbContext.Accounts.Any(x => x.Email == model.Email))
            {
                // send already registered error in email to prevent account enumeration
                SendAlreadyRegisteredEmail(model.Email, origin);
                return;
            }

            // map model to new account object
            var account = _mapper.Map<Account>(model);

            // first registered account is an admin
            var isFirstAccount = !_accountDbContext.Accounts.Any();
            account.Role = isFirstAccount ? Role.Super : Role.User;
            account.CreatedAt = DateTime.UtcNow;
            account.VerificationToken = RandomTokenString();

            // hash password
            account.PasswordHash = BC.HashPassword(model.Password);

            // save account
            _accountDbContext.Accounts.Add(account);
            _accountDbContext.SaveChanges();

            // send email
            SendVerificationEmail(account, origin);
        }

        public void VerifyEmail(string token)
        {
            var account = _accountDbContext.Accounts.SingleOrDefault(x => x.VerificationToken == token);

            if (account == null) throw new AppException("Verification failed");

            account.VerifiedAt = DateTime.UtcNow;
            account.VerificationToken = null;

            _accountDbContext.Accounts.Update(account);
            _accountDbContext.SaveChanges();
        }

        public void ResendVerificationToken(ResendVerificationTokenRequest model, string origin)
        {
            var account = _accountDbContext.Accounts.SingleOrDefault(x => x.Email == model.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return;

            // check if the account is already verified or not 
            if (account.IsVerified)
            {
                throw new AppException("This email address is already verified.");
            }

            account.VerificationToken = RandomTokenString();

            _accountDbContext.Accounts.Update(account);
            _accountDbContext.SaveChanges();

            // send email
            SendVerificationEmail(account, origin);
        }

        public void ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = _accountDbContext.Accounts.SingleOrDefault(x => x.Email == model.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return;

            // create reset token that expires after 1 day
            account.ResetToken = RandomTokenString();
            account.ResetTokenExpiresAt = DateTime.UtcNow.AddDays(1);

            _accountDbContext.Accounts.Update(account);
            _accountDbContext.SaveChanges();

            // send email
            SendPasswordResetEmail(account, origin);
        }

        public void ValidateResetToken(ValidateResetTokenRequest model)
        {
            var account = _accountDbContext.Accounts.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpiresAt > DateTime.UtcNow);

            if (account == null)
                throw new AppException("Invalid token");
        }

        public void ResetPassword(ResetPasswordRequest model)
        {
            var account = _accountDbContext.Accounts.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpiresAt > DateTime.UtcNow);

            if (account == null)
                throw new AppException("Invalid token");

            // update password and remove reset token
            account.PasswordHash = BC.HashPassword(model.Password);
            account.PasswordResetedAt = DateTime.UtcNow;
            account.ResetToken = null;
            account.ResetTokenExpiresAt = null;

            _accountDbContext.Accounts.Update(account);
            _accountDbContext.SaveChanges();
        }

        public IEnumerable<AccountResponse> GetAll()
        {
            var accounts = _accountDbContext.Accounts;
            return _mapper.Map<IList<AccountResponse>>(accounts);
        }

        public AccountResponse GetById(Guid id)
        {
            var account = GetAccount(id);
            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Create(CreateRequest model)
        {
            // validate
            if (_accountDbContext.Accounts.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");

            // map model to new account object
            var account = _mapper.Map<Account>(model);
            account.CreatedAt = DateTime.UtcNow;
            account.VerifiedAt = DateTime.UtcNow;

            // hash password
            account.PasswordHash = BC.HashPassword(model.Password);

            // save account
            _accountDbContext.Accounts.Add(account);
            _accountDbContext.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Update(Guid id, UpdateRequest model)
        {
            var account = GetAccount(id);

            // validate
            if (account.Email != model.Email && _accountDbContext.Accounts.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already taken");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                account.PasswordHash = BC.HashPassword(model.Password);

            // copy model to account and save
            _mapper.Map(model, account);
            account.UpdatedAt = DateTime.UtcNow;
            _accountDbContext.Accounts.Update(account);
            _accountDbContext.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public void Delete(Guid id)
        {
            var account = GetAccount(id);
            _accountDbContext.Accounts.Remove(account);
            _accountDbContext.SaveChanges();
        }

        // helper methods

        private Account GetAccount(Guid id)
        {
            var account = _accountDbContext.Accounts.Find(id);
            if (account == null) throw new KeyNotFoundException("Account not found");
            return account;
        }

        private (RefreshToken, Account) GetRefreshToken(string token)
        {
            var account = _accountDbContext.Accounts.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (account == null) throw new AppException("Invalid token");
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive) throw new AppException("Invalid token");
            return (refreshToken, account);
        }

        private string GenerateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private void RemoveOldRefreshTokens(Account account)
        {
            account.RefreshTokens.RemoveAll(x => 
                !x.IsActive && 
                x.CreatedAt.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private static string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private void SendVerificationEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var verifyUrl = $"{origin}/account/verify-email?token={account.VerificationToken}";
                message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                             <p><code>{account.VerificationToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Verify Email",
                html: $@"<h4>Verify Email</h4>
                         <p>Thanks for registering!</p>
                         {message}"
            );
        }

        private void SendAlreadyRegisteredEmail(string email, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
                message = $@"<p>If you don't know your password please visit the <a href=""{origin}/account/forgot-password"">forgot password</a> page.</p>";
            else
                message = "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

            _emailService.Send(
                to: email,
                subject: "Sign-up Verification API - Email Already Registered",
                html: $@"<h4>Email Already Registered</h4>
                         <p>Your email <strong>{email}</strong> is already registered.</p>
                         {message}"
            );
        }

        private void SendPasswordResetEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                             <p><code>{account.ResetToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                         {message}"
            );
        }
    }
}
