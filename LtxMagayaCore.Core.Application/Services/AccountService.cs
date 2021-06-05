using AutoMapper;
using LtxMagayaCore.Core.Application.Exceptions;
using LtxMagayaCore.Core.Application.Interfaces;
using LtxMagayaCore.Core.Application.mappings;
using LtxMagayaCore.Core.Domain;
using LtxMagayaCore.Core.Domain.Accounts;
using LtxMagayaCore.Infrastructure.Data;
using LtxMagayaCore.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace LtxMagayaCore.Core.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;
        private readonly IEntitiesService _magaya;
        public AccountService(
            DataContext context,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IEmailService emailService, IEntitiesService magaya)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _magaya = magaya;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

            if (account == null || !account.IsVerified || !BC.Verify(model.Password, account.PasswordHash))
                throw new AppException("Email or password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = generateJwtToken(account);
            var refreshToken = generateRefreshToken(ipAddress);
            account.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from account
            removeOldRefreshTokens(account);

            // save changes to db
            _context.Update(account);
            _context.SaveChanges();

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = refreshToken.Token;
            return response;
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var (refreshToken, account) = getRefreshToken(token);

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            account.RefreshTokens.Add(newRefreshToken);

            removeOldRefreshTokens(account);

            _context.Update(account);
            _context.SaveChanges();

            // generate new jwt
            var jwtToken = generateJwtToken(account);

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = newRefreshToken.Token;
            return response;
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var (refreshToken, account) = getRefreshToken(token);

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(account);
            _context.SaveChanges();
        }

        public void Register(RegisterRequest model, string origin)
        {
            // validate
            if (_context.Accounts.Any(x => x.Email == model.Email))
            {
                // send already registered error in email to prevent account enumeration
                sendAlreadyRegisteredEmail(model.Email, origin);
                return;
            }




            // map model to new account object
            var mapperRequest = GeneralProfile.InitializeAutomapper();
            var account = mapperRequest.Map<Account>(model);


            // first registered account is an admin
            var isFirstAccount = _context.Accounts.Count() == 0;
            account.AccountPhone = model.phone;
            account.Role = isFirstAccount ? Role.Admin : Role.User;
            account.Created = DateTime.UtcNow;
            account.VerificationToken = randomTokenString();

            // hash password
            account.PasswordHash = BC.HashPassword(model.Password);







            // save account
            _context.Accounts.Add(account);
            _context.SaveChanges();

            // send email
            sendVerificationEmail(account, origin);
        }

        public void VerifyEmail(string token)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.VerificationToken == token);

            if (account == null) throw new AppException("Verification failed");

            account.Verified = DateTime.UtcNow;
            account.VerificationToken = null;

            _magaya.CrearEntidad(account);
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public void ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return;

            // create reset token that expires after 1 day
            account.ResetToken = randomTokenString();
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            _context.Accounts.Update(account);
            _context.SaveChanges();

            // send email
            sendPasswordResetEmail(account, origin);
        }

        public void ValidateResetToken(ValidateResetTokenRequest model)
        {
            var account = _context.Accounts.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
                throw new AppException("Invalid token");
        }

        public void ResetPassword(ResetPasswordRequest model)
        {
            var account = _context.Accounts.SingleOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
                throw new AppException("Invalid token");

            // update password and remove reset token
            account.PasswordHash = BC.HashPassword(model.Password);
            account.PasswordReset = DateTime.UtcNow;
            account.ResetToken = null;
            account.ResetTokenExpires = null;

            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public IEnumerable<AccountResponse> GetAll()
        {
            var accounts = _context.Accounts;
            return _mapper.Map<IList<AccountResponse>>(accounts);
        }

        public AccountResponse GetById(int id)
        {
            var account = getAccount(id);
            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Create(CreateRequest model)
        {
            // validate
            if (_context.Accounts.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already registered");

            // map model to new account object
            var account = _mapper.Map<Account>(model);
            account.Created = DateTime.UtcNow;
            account.Verified = DateTime.UtcNow;

            // hash password
            account.PasswordHash = BC.HashPassword(model.Password);

            // save account
            _context.Accounts.Add(account);
            _context.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Update(int id, UpdateRequest model)
        {
            var account = getAccount(id);

            // validate
            if (account.Email != model.Email && _context.Accounts.Any(x => x.Email == model.Email))
                throw new AppException($"Email '{model.Email}' is already taken");

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
                account.PasswordHash = BC.HashPassword(model.Password);

            // copy model to account and save
            _mapper.Map(model, account);
            account.Updated = DateTime.UtcNow;
            _context.Accounts.Update(account);
            _context.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public void Delete(int id)
        {
            var account = getAccount(id);
            _context.Accounts.Remove(account);
            _context.SaveChanges();
        }

        // helper methods

        private Account getAccount(int id)
        {
            var account = _context.Accounts.Find(id);
            if (account == null) throw new KeyNotFoundException("Account not found");
            return account;
        }

        private (RefreshToken, Account) getRefreshToken(string token)
        {
            var account = _context.Accounts.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (account == null) throw new AppException("Invalid token");
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive) throw new AppException("Invalid token");
            return (refreshToken, account);
        }

        private string generateJwtToken(Account account)
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

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = randomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private void removeOldRefreshTokens(Account account)
        {
            account.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private string randomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private void sendVerificationEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var verifyUrl = $"{origin}/account/verify-email?token={account.VerificationToken}";
                message = $@"<a href=""{verifyUrl}"">Validar mi registro.</a>";
            }
            else
            {
                message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                             <p><code>{account.VerificationToken}</code></p>";
            }
            string body = "<table width='660' border='0' align='center' cellpadding='1' cellspacing='0' bgcolor='#CCCCCC'><tr><td>";
            body += "<table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td height='130' align='center' bgcolor='#f5f6f7'>";
            body += "<img src='http://ltx.com.ve/images/logo-ltx---final-201912.png' width='231' height='72' /></td></tr></table></td></tr><tr><td>";
            body += "<table border='0' align='center' cellpadding='30' cellspacing='0'><tr><td valign='top' bgcolor='#FFFFFF'>";
            body += "<h1 style='font-family: Verdana, Geneva, sans-serif;font-size: 22px; color: #333333; font-weight: bold; letter-spacing: -1px;'>";
            body += "</h1><p><span style='font-family: Verdana, Geneva, sans-serif; font-size: 13px; color: #000000;'>";
            body += "Gracias por escogernos como tu courier de confianza. Para continuar con el proceso de registro haz clic aqu&iacute;:</span><br /><br /></p>";
            body += "<table width='250' border='0' align='center' cellpadding='15' cellspacing='0'><tr><td align='center'>" + message + " </td></tr></table><p>&nbsp;</p>";
            body += "<br /></td></tr><tr>";
            body += "<td valign='top' bgcolor='#FFFFFF'><table width='100%' border='0' cellspacing='0' cellpadding='0'><tr><td></td><td align='right'>";
            body += "<img src='http://www.ltx.com.ve/app/img/tetraemos.jpg' width='201' height='16' /></td></tr></table></td></tr></table></td></tr><tr><td>";
            body += "<table width='100%' border='0' cellspacing='0' cellpadding='30'><tr><td bgcolor='#FFFFFF'>";
            body += "<p style='font-family: Verdana, Geneva, sans-serif; font-size: 10px; color: #333333;'><strong>";
            body += "Por favor no responder a este mensaje ya que el mismo fue enviado desde una cuenta de correo electr&oacute;nico no monitoreada. </strong>";
            body += "</p><p style='font-family: Verdana, Geneva, sans-serif; font-size: 10px; color: #333333;' dir='ltr'>";
            body += "Tu contrase&ntilde;a es confidencial, LTX® nunca solicitar&aacute; datos de tu cuenta y contrase&ntilde;a por correo electr&oacute;nico ni por ning&uacute;n otro medio.</p><p dir='ltr'>";
            body += "<strong style='font-family: Verdana, Geneva, sans-serif; font-size: 10px; color: #333333;' id='docs-internal-guid-76b42a8c-6675-d983-e14a-bd00f1c486aa'>";
            body += "¿Dudas? escribenos a info@ltx.com.ve y te ayudaremos</strong></p></td></tr></table></td></tr><tr><td align='center'>";
            body += "<table width='100%' border='0' cellspacing='0' cellpadding='20' style='font-family: Verdana, Geneva, sans-serif; font-size: 13px; color: #FFFFFF; text-decoration: none;'>";
            body += "<tr><td align='center' valign='top' bgcolor='#21619D' style='font-family: Verdana, Geneva, sans-serif; font-size: 13px; color: #FFFFFF; text-decoration: none;'>";
            body += "<strong>Miami - Estados Unidos</strong><br />8000 NW 29th St,<br />Miami,FL 33122<br /><br /><a href='#' style='color:#FFF; text-decoration:none'>+1(305)640-5391</a><br /><a href='#' style='color:#FFF; text-decoration:none'>+1(305)640-5639</a></td><td align='center' valign='top' bgcolor='#21619D'><strong>Caracas - Venezuela</strong><br /><a href='#' style='color:#FFF; text-decoration:none'>+58(212)655-0603</a><br />RIF:J-40620006-9 <br /><br /><br /><a href='http://www.ltx.com.ve' style='font-family: Verdana, Geneva, sans-serif; font-size: 13px; color: #FFFFFF; text-decoration: none;'><strong>www.ltx.com.ve</strong></a></td><td width='30%' align='center' valign='top' bgcolor='#21619D'><strong>Síguenos en:</strong><br /><br /><table width='100%' border='0' cellspacing='0' cellpadding='2'><tr><td><a href='https://www.facebook.com/Latam-Xpress-1049926855020369/?fref=ts' target='_blank'><img src='http://www.latamxpress.com/app/img/facebook.png' width='38' height='38' /></a></td><td><a href='https://www.instagram.com/latamxpress/' target='_blank'><img src='http://www.latamxpress.com/app/img/instagram.png' width='39' height='38' /></a></td><td><a href='https://twitter.com/latamxpress' target='_blank'><img src='http://www.latamxpress.com/app/img/twitter.png' width='38' height='38' /></a></td><td><a href='https://www.youtube.com/channel/UCfHfXf1oNSKP9wlPNn9_TwA' target='_blank'><img src='http://www.latamxpress.com/app/img/youtube.png' width='38' height='38' /></a></td></tr></table></td></tr></table></td></tr></table>";


            _emailService.Send(
                to: account.Email,
                subject: "REGISTRO EN LTX.COM.VE",
                html: body
            );
        }

        private void sendAlreadyRegisteredEmail(string email, string origin)
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

        private void sendPasswordResetEmail(Account account, string origin)
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
