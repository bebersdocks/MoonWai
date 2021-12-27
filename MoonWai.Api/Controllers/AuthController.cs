using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

using LinqToDB;

using MoonWai.Api.Definitions;
using MoonWai.Api.Models.Auth;
using MoonWai.Api.Models.User;
using MoonWai.Api.Utils;
using MoonWai.Dal;
using MoonWai.Dal.DataModels;

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
                return BadRequest(ErrorId.UsernameCantBeEmpty);

            using var dc = new Dc();

            var user = await dc.Users
                .LoadWith(i => i.Settings.DefaultBoard)
                .FirstOrDefaultAsync(i => i.Username == loginDto.Username);

            if (user == null)
                return NotFound(ErrorId.UserNotFound);

            if (!Crypto.ValidatePassword(loginDto.Password, user.PasswordSalt, user.PasswordHash))
                return LogonFailed(ErrorId.WrongPassword);

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
                return BadRequest(ErrorId.UsernameCantBeEmpty);

            using var dc = new Dc();
            
            if (await dc.Users.AnyAsync(i => i.Username == registerDto.Username))
                return Conflict(ErrorId.UserIsAlreadyRegistered);

            if (string.IsNullOrEmpty(registerDto.Password))
                return BadRequest(ErrorId.PasswordCantBeEmpty);

            if (registerDto.Password.Length < Constants.MinPasswordLength)
                return BadRequest(ErrorId.PasswordLengthCantBeLessThan, Constants.MinPasswordLength);
            
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
                return ServerError(ErrorId.FailedToCreateNewUser);

            userSettings.UserId = userId;
            userSettings.DefaultBoardId = Constants.DefaultBoardId;

            if (await dc.InsertAsync(userSettings) <= 0)
                return ServerError(ErrorId.FailedToCreateUserSettings);
   
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
