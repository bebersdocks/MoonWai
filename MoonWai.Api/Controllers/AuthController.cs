using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using LinqToDB;

using MoonWai.Api.Resources.Translations;
using MoonWai.Api.Utils;
using MoonWai.Dal;
using MoonWai.Dal.DataModels;
using MoonWai.Shared.Auth;
using MoonWai.Shared.Definitions;

namespace MoonWai.Api.Controllers
{
    public class AuthController : BaseController
    {
        public AuthController(ILogger<AuthController> logger) : base(logger) { }

        [NonAction]
        public Task Login(User user, bool trusted = false)
        {
            var claims = new Claim[]
            {   
                new("UserId", user.UserId.ToString()),
                new(ClaimTypes.Name, user.Username) 
            };

            var claimsIdentity = new ClaimsIdentity(claims, Program.defaultAuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.Add(trusted ? TimeSpan.FromDays(30) : TimeSpan.FromHours(72)),
            };

            return HttpContext.SignInAsync(Program.defaultAuthenticationScheme, claimsPrincipal, authProperties);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Username))
                return BadRequest(TranslationId.UsernameCantBeEmpty);

            using var dc = new Dc();

            var user = await dc.Users.FirstOrDefaultAsync(i => i.Username == loginDto.Username);

            if (user == null)
                return NotFound(TranslationId.UserNotFound);

            if (!Crypto.ValidatePassword(loginDto.Password, user.PasswordSalt, user.PasswordHash))
                return BadRequest(TranslationId.WrongPassword);

            user.LastAccessDt = DateTime.UtcNow;

            await dc.UpdateAsync(user);

            await Login(user, trusted: loginDto.Trusted);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (string.IsNullOrEmpty(registerDto.Username))
                return BadRequest(TranslationId.UsernameCantBeEmpty);

            using var dc = new Dc();
            
            if (await dc.Users.AnyAsync(i => i.Username == registerDto.Username))
                return BadRequest(TranslationId.UserIsAlreadyRegistered);

            if (string.IsNullOrEmpty(registerDto.Password))
                return BadRequest(TranslationId.PasswordCantBeEmpty);

            if (registerDto.Password.Length < Constants.MIN_PASSWORD_LENGTH)
                return BadRequest(TranslationId.PasswordLengthCantBeLessThan, Constants.MIN_PASSWORD_LENGTH);
            
            (var salt, var hash) = Crypto.GenerateSaltHash(registerDto.Password);

            var newUser = new User();

            newUser.Username = registerDto.Username;
            newUser.PasswordSalt = salt;
            newUser.PasswordHash = hash;

            var utcNow = DateTime.UtcNow;

            newUser.CreateDt = utcNow;
            newUser.LastAccessDt = utcNow;
            
            var userSettings = new UserSettings();

            userSettings.LanguageId = registerDto.LanguageId;

            await dc.BeginTransactionAsync();

            var userId = await dc.InsertWithInt32IdentityAsync(newUser);

            if (userId <= 0)
                return BadRequest(TranslationId.FailedToCreateNewUser);

            userSettings.UserId = userId;

            if (await dc.InsertAsync(userSettings) <= 0)
                return BadRequest(TranslationId.FailedToCreateUserSettings);
   
            await dc.CommitTransactionAsync();

            await Login(newUser);

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(Program.defaultAuthenticationScheme);

            return Ok();
        }
    }
}
