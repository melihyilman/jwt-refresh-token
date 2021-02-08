using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace userApp.Models
{
    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(User user, string jwtToken, string refreshToken)
        {
            Id = user.id;
            FullName = user.name;
            UserName = user.username;
            Email = user.email;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
