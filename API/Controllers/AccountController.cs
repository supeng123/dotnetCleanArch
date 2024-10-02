using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Constants;
using Identity.Data;
using Identity.DTOs.Accounts;
using Identity.Model;
using Identity.Services.AuthServices;
using Identity.Services.IAuthServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly Context _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly IJWTService _jwtService;

        public AccountController(
            Context context,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            EmailService emailService,
            IJWTService jwtService
            )
        {
            this._context = context;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._configuration = configuration;
            this._emailService = emailService;
            this._jwtService = jwtService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (await CheckEmailExistsAsync(model.Email))
            {
                return BadRequest($"An existing account is using {model.Email}, email addres. Please try with another email address");
            }

            var userToAdd = new User
            {
                Firstname = model.FirstName.ToLower(),
                Lastname = model.LastName.ToLower(),
                UserName = model.Email.ToLower(),
                Email = model.Email.ToLower(),
                EmailConfirmed = true,
            };

            // creates a user inside our AspNetUsers table inside our database
            var result = await _userManager.CreateAsync(userToAdd, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            await _userManager.AddToRoleAsync(userToAdd, SD.PlayerRole);

            return Ok(new JsonResult(new { title = "Account Created", message = "Your account has been created, please confrim your email address" }));

            // try
            // {
            //     // if (await SendConfirmEMailAsync(userToAdd))
            //     // {
            //         return Ok(new JsonResult(new { title = "Account Created", message = "Your account has been created, please confrim your email address" }));
            //     // }

            //     return BadRequest("Failed to send email. Please contact admin");
            // }
            // catch (Exception)
            // {
            //     return BadRequest("Failed to send email. Please contact admin");
            // }

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return Unauthorized("Invalid username or password");

            if (user.EmailConfirmed == false) return Unauthorized("Please confirm your email.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.IsLockedOut)
            {
                return Unauthorized(string.Format("Your account has been locked. You should wait until {0} (UTC time) to be able to login", user.LockoutEnd));
            }

            if (!result.Succeeded)
            {
                // User has input an invalid password
                if (!user.UserName.Equals(SD.AdminUserName))
                {
                    // Increamenting AccessFailedCount of the AspNetUser by 1
                    await _userManager.AccessFailedAsync(user);
                }

                if (user.AccessFailedCount >= SD.MaximumLoginAttempts)
                {
                    // Lock the user for one day
                    await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(1));
                    return Unauthorized(string.Format("Your account has been locked. You should wait until {0} (UTC time) to be able to login", user.LockoutEnd));
                }


                return Unauthorized("Invalid username or password");
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            await _userManager.SetLockoutEndDateAsync(user, null);

            return await CreateApplicationUserDto(user);
        }

        private async Task<UserDto> CreateApplicationUserDto(User user)
        {
            await SaveRefreshTokenAsync(user);
            return new UserDto
            {
                FirstName = user.Firstname,
                LastName = user.Lastname,
                JWT = await _jwtService.CreateJWT(user),
            };
        }

        private async Task SaveRefreshTokenAsync(User user)
        {
            var refreshToken = _jwtService.CreateRefreshToken(user);

            var existingRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.UserId == user.Id);
            if (existingRefreshToken != null)
            {
                existingRefreshToken.Token = refreshToken.Token;
                existingRefreshToken.DateCreatedUtc = refreshToken.DateCreatedUtc;
                existingRefreshToken.DateExpiresUtc = refreshToken.DateExpiresUtc;
            }
            else
            {
                user.RefreshTokens.Add(refreshToken);
            }

            await _context.SaveChangesAsync();

            var cookieOptions = new CookieOptions
            {
                Expires = refreshToken.DateExpiresUtc,
                IsEssential = true,
                HttpOnly = true,
            };

            Response.Cookies.Append("identityAppRefreshToken", refreshToken.Token, cookieOptions);
        }

        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }

        private async Task<bool> SendConfirmEMailAsync(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{_configuration["JWT:ClientUrl"]}/{_configuration["Email:ConfirmEmailPath"]}?token={token}&email={user.Email}";

            var body = $"<p>Hello: {user.Firstname} {user.Lastname}</p>" +
                "<p>Please confirm your email address by clicking on the following link.</p>" +
                $"<p><a href=\"{url}\">Click here</a></p>" +
                "<p>Thank you,</p>" +
                $"<br>{_configuration["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(user.Email, "Confirm your email", body);

            return await _emailService.SendEmailAsync(emailSend);
        }

    }


}