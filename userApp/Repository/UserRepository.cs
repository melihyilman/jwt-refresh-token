using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Data;
using Npgsql;
using userApp.Models;
using System;
using userApp.Models.interfaces;
using userApp.Utils;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

namespace userApp.Repository
{
    public class UserRepository : IRepository<User>,IUserService
    {
        private string connectionString;
        private readonly AppSettings _appSettings;
        public UserRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetValue<string>("DBInfo:ConnectionString");

           // _appSettings = appSettings.Value;
        }

        internal IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(connectionString);
            }
        }

        public void Add(User item)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("INSERT INTO Users (name,username,email,password) VALUES(@name,@username,@email,@password)", item);
            }

        }

        public IEnumerable<User> FindAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM Users");
            }
        }

        public User FindByID(Guid id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM Users WHERE id = @id", new { id = id }).FirstOrDefault();
            }
        }
        public User FindByUserName(string username)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM Users WHERE username = @username", new { username = username }).FirstOrDefault();
            }
        }
        public User FindByToken(string token)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM Users WHERE token = @token", new { token = token }).FirstOrDefault();
            }
        }

        public void Remove(Guid id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute("DELETE FROM Users WHERE Id=@Id", new { Id = id });
            }
        }

        public void Update(User item)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Query("UPDATE Users SET name = @name,  username  = @username, email= @email, password= @password, token= @token, token_expiration= @token_expiration, refresh_token_expiration= @refresh_token_expiration, refresh_token= @refresh_token WHERE id = @id", item);
            }
        }
        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = FindByUserName(model.Username);
            if (user == null || user.password != model.Password)
                return null;

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = generateJwtToken(user);
            var refreshToken = generateRefreshToken();

            // save refresh token
            user.token = refreshToken.Token;
            user.token_expiration = refreshToken.Expires;
            user.refresh_token_expiration = refreshToken.Expires;
            user.refresh_token = refreshToken.Token;
            Update(user);

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }
        public AuthenticateResponse Authenticate(string username,string password)
        {
            var user = FindByUserName(username);
            if (user == null || user.password != password)
                return null;

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = generateJwtToken(user);
            var refreshToken = generateRefreshToken();

            // save refresh token
            user.token = refreshToken.Token;
            user.token_expiration = refreshToken.Expires;
            user.refresh_token_expiration = refreshToken.Expires;
            user.refresh_token = refreshToken.Token;
            Update(user);

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }


        public AuthenticateResponse RefreshToken(string token)
        {
            var user = FindByToken(token);
            if (user == null)
                return null;

            // return null if token is no longer active
            if (!user.IsExpired) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken();

            user.token = newRefreshToken.Token;
            user.token_expiration = newRefreshToken.Expires;
            user.refresh_token_expiration = newRefreshToken.Expires;
            user.refresh_token_expiration = newRefreshToken.Expires;
            Update(user);
            // generate new jwt
            var jwtToken = generateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public bool RevokeToken(string token)
        {
            var user = FindByToken(token);

            // return false if no user found with token
            if (user == null) return false;

            // return false if token is not active
            if (!user.IsExpired) return false;

            // revoke token and save
            user.refresh_token_expiration = DateTime.UtcNow;
            user.token_expiration = DateTime.UtcNow;
            //refreshToken.RevokedByIp = ipAddress;
            Update(user);

            return true;
        }
        private string generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("This is very secret key");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                };
            }
        }
    }
}
