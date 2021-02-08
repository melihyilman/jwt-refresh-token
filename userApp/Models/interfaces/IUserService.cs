using System;
using System.Collections.Generic;

namespace userApp.Models.interfaces{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        AuthenticateResponse Authenticate(string username,string password);
        AuthenticateResponse RefreshToken(string token);
        bool RevokeToken(string token);
        User FindByID(Guid id);
    }

}