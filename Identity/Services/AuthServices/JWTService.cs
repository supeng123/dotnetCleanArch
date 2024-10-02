using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Identity.Model;
using Identity.Services.IAuthServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Services.AuthServices
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public JWTService(IConfiguration configuration, UserManager<User> userManager)
        {
            this._configuration = configuration;
            this._userManager = userManager;
        }
        public async Task<string> CreateJWT(User user)
        {
            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.UserName!),
                new Claim(ClaimTypes.GivenName, user.Firstname),
                new Claim(ClaimTypes.Surname, user.Lastname)
            };

            var roles = await _userManager.GetRolesAsync(user);
            userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var creadentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JWT:ExpiresInMinutes"]!)),
                SigningCredentials = creadentials,
                Issuer = _configuration["JWT:Issuer"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(jwt);
        }

        public RefreshToken CreateRefreshToken(User user)
        {
            var token = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(token);

            var refreshToken = new RefreshToken()
            {
                Token = Convert.ToBase64String(token),
                User = user,
                DateExpiresUtc = DateTime.UtcNow.AddDays(int.Parse(_configuration["JWT:RefreshTokenExpiresInDays"]))
            };

            return refreshToken;
        }
    }
}