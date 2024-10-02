using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Data;
using Identity.Model;
using Identity.Services.AuthServices;
using Identity.Services.IAuthServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Identity
{
    public static class IdentityServiceRegistration
    {
        public static IServiceCollection ConfigureIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Context>(options =>
               options.UseSqlServer(
                   configuration.GetConnectionString("LoganIdentityConnectionString")));
            services.AddIdentityCore<User>(options =>
            {
                // password configuration
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                // for email confirmation
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<IdentityRole>() // be able to add roles
            .AddRoleManager<RoleManager<IdentityRole>>() // be able to make use of RoleManager
            .AddEntityFrameworkStores<Context>() // providing our context
            .AddSignInManager<SignInManager<User>>() // make use of Signin manager
            .AddUserManager<UserManager<User>>() // make use of UserManager to create users
            .AddDefaultTokenProviders(); // be able to create tokens for email confirmation

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // validate the token based on the key we have provided inside appsettings.development.json JWT:Key
                    ValidateIssuerSigningKey = true,
                    // the issuer singning key based on JWT:Key
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JWT:Key").Value!)),
                    // the issuer which in here is the api project url we are using
                    ValidIssuer = configuration.GetSection("JWT:Issuer").Value,
                    // validate the issuer (who ever is issuing the JWT)
                    ValidateIssuer = true,
                    // don't validate audience (angular side)
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            }); // be able to use authentication

            services.AddScoped<IJWTService, JWTService>();
            services.AddScoped<EmailService>();
            services.AddScoped<ContextSeedingService>();

            return services;
        }

    }
}