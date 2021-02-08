using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace userApp.Models
{
    public abstract class BaseEntity
    {
    }
    public class User : BaseEntity
    {
        public Guid id { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public string username { get; set; }
        [JsonIgnore]
        public string password { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string salt { get; set; }
        public string token { get; set; }
        public string refresh_token { get; set; }
        public DateTime token_expiration { get; set; }
        public DateTime refresh_token_expiration { get; set; }
        public bool IsExpired => DateTime.UtcNow >= token_expiration;
        //[JsonIgnore]
        //public List<RefreshToken> RefreshTokens { get; set; }
    }
}
