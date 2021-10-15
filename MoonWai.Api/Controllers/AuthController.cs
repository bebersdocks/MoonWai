using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using LinqToDB;

using MoonWai.Api.Resources;
using MoonWai.Api.Utils;
using MoonWai.Dal;
using MoonWai.Dal.DataModels;
using MoonWai.Shared.Definitions;
using MoonWai.Shared.Models;

namespace MoonWai.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController : BaseController
    {
        public AuthController(ILogger<AuthController> logger) : base(logger) { }

        [NonAction]
        public Task Login(User user, bool trusted = false)
        {
            var claims = new Claim[]
            {   
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.NameIdentifier, user.UserId.ToString())
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

            var user = await
                dc.Users
                    .LoadWith(i => i.Settings.DefaultBoard)
                    .FirstOrDefaultAsync(i => i.Username == loginDto.Username);

            if (user == null)
                return NotFound(TranslationId.UserNotFound);

            if (!Crypto.ValidatePassword(loginDto.Password, user.PasswordSalt, user.PasswordHash))
                return LogonFailed(TranslationId.WrongPassword);

            user.LastAccessDt = DateTime.UtcNow;

            await dc.UpdateAsync(user);

            await Login(user, trusted: loginDto.Trusted);

            return Ok(new UserSettingsDto(user.Settings.LanguageId, user.Settings.DefaultBoard.Path));
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (string.IsNullOrEmpty(registerDto.Username))
                return BadRequest(TranslationId.UsernameCantBeEmpty);

            using var dc = new Dc();
            
            if (await dc.Users.AnyAsync(i => i.Username == registerDto.Username))
                return Conflict(TranslationId.UserIsAlreadyRegistered);

            if (string.IsNullOrEmpty(registerDto.Password))
                return BadRequest(TranslationId.PasswordCantBeEmpty);

            if (registerDto.Password.Length < Common.minPasswordLength)
                return BadRequest(TranslationId.PasswordLengthCantBeLessThan, Common.minPasswordLength);
            
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
                return ServerError(TranslationId.FailedToCreateNewUser);

            userSettings.UserId = userId;
            userSettings.DefaultBoardId = Common.defaultBoardId;

            if (await dc.InsertAsync(userSettings) <= 0)
                return ServerError(TranslationId.FailedToCreateUserSettings);
   
            await dc.CommitTransactionAsync();

            await Login(newUser);

            var defaultBoard = await dc.Boards.FirstAsync(i => i.BoardId == Common.defaultBoardId);

            return Ok(new UserSettingsDto(userSettings.LanguageId, defaultBoard.Path));
        }

        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(Program.defaultAuthenticationScheme);

            return Ok();
        }
    }
}
