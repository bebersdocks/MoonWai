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

namespace MoonWai.Api.Controllers
{
    public class AuthController : BaseController
    {
        public AuthController(ILogger<AuthController> logger) : base(logger) { }

        [NonAction]
        public async Task Login(string username, bool trusted = false)
        {
            var claims = new Claim[] { new(ClaimTypes.Name, username) };
            var claimsIdentity = new ClaimsIdentity(claims, Program.defaultAuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.Add(trusted ? TimeSpan.FromDays(30) : TimeSpan.FromHours(72)),
            };

            await HttpContext.SignInAsync(Program.defaultAuthenticationScheme, claimsPrincipal, authProperties);
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool trusted = false)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest(TranslationId.UsernameCantBeEmpty);

            using var dc = new Dc();

            var user = await dc.Users.FirstOrDefaultAsync(i => i.Username == username);

            if (user == null)
                return NotFound(TranslationId.UserNotFound);

            if (!Crypto.ValidatePassword(password, user.PasswordSalt, user.PasswordHash))
                return BadRequest(TranslationId.WrongPassword);

            await Login(username, trusted: trusted);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, LanguageId? languageId = null)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest(TranslationId.UsernameCantBeEmpty);

            using var dc = new Dc();
            
            if (await dc.Users.AnyAsync(i => i.Username == username))
                return BadRequest(TranslationId.UserIsAlreadyRegistered);

            if (string.IsNullOrEmpty(password))
                return BadRequest(TranslationId.PasswordCantBeEmpty);

            if (password.Length < 8)
                return BadRequest(TranslationId.PasswordLengthCantBeLessThan, 8);
            
            (var salt, var hash) = Crypto.GenerateSaltHash(password);

            var newUser = new User();

            newUser.Username = username;
            newUser.PasswordSalt = salt;
            newUser.PasswordHash = hash;
            newUser.LanguageId = languageId ?? LanguageId.English;

            var utcNow = DateTime.UtcNow;

            newUser.CreateDt = utcNow;
            newUser.LastAccessDt = utcNow;
            
            if (dc.Insert(newUser) < 1)
                return BadRequest(TranslationId.FailedToCreateNewUser);

            await Login(newUser.Username);

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