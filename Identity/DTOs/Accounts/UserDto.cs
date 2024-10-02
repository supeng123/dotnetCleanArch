using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.DTOs.Accounts
{
    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JWT { get; set; }
    }
}