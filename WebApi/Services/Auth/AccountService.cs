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
using Microsoft.Extensions.Localization;
using System.Globalization;

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
        private readonly IStringLocalizer<AccountService> _localizer;

        public AccountService(
            AccountDbContext accountDbContext,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IEmailService emailService,
            IStringLocalizer<AccountService> localizer)
        {
            _accountDbContext = accountDbContext;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _localizer = localizer;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var account = _accountDbContext.Accounts.SingleOrDefault(x => x.Email == model.Email);

            if (account == null || !account.IsVerified || !BC.Verify(model.Password, account.PasswordHash))
                throw new AppException(_localizer["EmailPassIncorrect"].Value, 400, "BadRequest", _localizer["BadRequest"].Value);

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

            if (account == null) throw new AppException(_localizer["VerificationFailed"].Value, 400, "BadRequest", _localizer["BadRequest"].Value);

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
                throw new AppException(_localizer["EmailVerified"].Value, 400, "BadRequest", _localizer["BadRequest"].Value);
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
                throw new AppException(_localizer["InvalidToken"].Value, 400, "BadRequest", _localizer["BadRequest"].Value);
        }

        public void ResetPassword(ResetPasswordRequest model)
        {
            var account = _accountDbContext.Accounts.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpiresAt > DateTime.UtcNow);

            if (account == null)
                throw new AppException(_localizer["InvalidToken"].Value, 400, "BadRequest", _localizer["BadRequest"].Value);

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
                throw new AppException(_localizer["EmailRegistered", model.Email].Value, 400, "BadRequest", _localizer["BadRequest"].Value);

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
                throw new AppException(_localizer["EmailTaken", model.Email].Value, 400, "BadRequest", _localizer["BadRequest"].Value);

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
            if (account == null) throw new KeyNotFoundException(_localizer["AccountNotFound"].Value);
            return account;
        }

        private (RefreshToken, Account) GetRefreshToken(string token)
        {
            var account = _accountDbContext.Accounts.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (account == null) throw new AppException(_localizer["InvalidToken"].Value, 400, "BadRequest", _localizer["BadRequest"].Value);
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive) throw new AppException(_localizer["InvalidToken"].Value, 400, "BadRequest", _localizer["BadRequest"].Value);
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
            var randomBytes = RandomNumberGenerator.GetBytes(40);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private void SendVerificationEmail(Account account, string origin)
        {
            string message, uri, token = account.VerificationToken, culture = CultureInfo.CurrentCulture.Name;
            if (!string.IsNullOrEmpty(origin))
            {
                uri = $"{origin}/{culture}/Accounts/verify-email?token={token}";
                message = _localizer["VerifyLink", uri];
            }
            else
            {
                uri = $"/{culture}/Accounts/verify-email";
                message = _localizer["VerifyInstruction", uri, token];
            }

            _emailService.Send(
                to: account.Email,
                subject: _localizer["VerifyEmailSubject"],
                html: _localizer["VerifyEmailHeader"] + message
            );
        }

        private void SendAlreadyRegisteredEmail(string email, string origin)
        {
            string message, uri, culture = CultureInfo.CurrentCulture.Name;
            if (!string.IsNullOrEmpty(origin))
            {
                uri = $"{origin}/{culture}/Accounts/forgot-password";
                message = _localizer["AlreadyRegisteredEmail", uri];
            }
            else
            {
                uri = $"/{culture}/Accounts/forgot-password";
                message = _localizer["AlreadyRegisteredInstruction", uri];
            }

            _emailService.Send(
                to: email,
                subject: _localizer["AlreadyRegisteredEmailSubject"],
                html: _localizer["AlreadyRegisteredEmailHeader", email] + message
            );
        }

        private void SendPasswordResetEmail(Account account, string origin)
        {
            string message, uri, token = account.ResetToken, culture = CultureInfo.CurrentCulture.Name;
            if (!string.IsNullOrEmpty(origin))
            {
                uri = $"{origin}/{culture}/Accounts/reset-password?token={token}";
                message = _localizer["PasswordResetLink", uri];
            }
            else
            {
                uri = $"/{culture}/Accounts/reset-password";
                message = _localizer["PasswordResetInstruction", uri, token];
            }

            _emailService.Send(
                to: account.Email,
                subject: _localizer["PasswordResetEmailSubject"],
                html: _localizer["PasswordResetEmailHeader"] + message
            );
        }
    }
}
