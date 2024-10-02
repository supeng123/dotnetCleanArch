using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Model;
using Microsoft.Identity.Client;

namespace Identity.Services.IAuthServices
{
    public interface IJWTService
    {
        Task<string> CreateJWT(User user);

        RefreshToken CreateRefreshToken(User user);
    }
}