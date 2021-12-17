using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

using LinqToDB;

using MoonWai.Api.Utils;
using MoonWai.Dal;
using MoonWai.Dal.DataModels;
using MoonWai.Shared;
using MoonWai.Shared.Definitions;
using MoonWai.Shared.Models.Auth;
using MoonWai.Shared.Models.User;

namespace MoonWai.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : BaseController
    {
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

            var defaultBoard = user.Settings.DefaultBoard;

            return Ok(new UserDto { Username = user.Username, DefaultBoardPath = defaultBoard.Path });
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

            if (registerDto.Password.Length < Constants.MinPasswordLength)
                return BadRequest(TranslationId.PasswordLengthCantBeLessThan, Constants.MinPasswordLength);
            
            (var salt, var hash) = Crypto.GenerateSaltHash(registerDto.Password);

            var newUser = new User();

            newUser.Username = registerDto.Username;
            newUser.PasswordSalt = salt;
            newUser.PasswordHash = hash;

            newUser.AccessLevel = AccessLevel.Standard;

            var utcNow = DateTime.UtcNow;

            newUser.CreateDt = utcNow;
            newUser.LastAccessDt = utcNow;
            
            var userSettings = new UserSettings();

            userSettings.LanguageId = LanguageId.English;

            using var tr = await dc.BeginTransactionAsync();

            var userId = await dc.InsertWithInt32IdentityAsync(newUser);

            if (userId <= 0)
                return ServerError(TranslationId.FailedToCreateNewUser);

            userSettings.UserId = userId;
            userSettings.DefaultBoardId = Constants.DefaultBoardId;

            if (await dc.InsertAsync(userSettings) <= 0)
                return ServerError(TranslationId.FailedToCreateUserSettings);
   
            await tr.CommitAsync();

            await Login(newUser);

            var defaultBoard = await dc.Boards.FirstAsync(i => i.BoardId == Constants.DefaultBoardId);

            return Ok(new UserDto { Username = newUser.Username, DefaultBoardPath = defaultBoard.Path });
        }

        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(Program.defaultAuthenticationScheme);

            return Ok();
        }
    }
}
