using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallPosters.Data;
using SmallPosters.Models;
using SmallPosters.Models.Cryptography;
using SmallPosters.Web.DTO;

namespace SmallPosters.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        const string DefaultAdminUsername = "admin";
        const string DefaultAdminPassword = "adminPassword";
        //Note: the above should be removed in production
        const int TokenSize = 10;
        private readonly SmallPostersContext _smallPostersContext;
        public UserController(SmallPostersContext smallPostersContext)
        {
            this._smallPostersContext = smallPostersContext;
            if (!_smallPostersContext.Accounts.Any(a=>a.Username==DefaultAdminUsername))
            {
                _smallPostersContext.Accounts.Add(new Account(DefaultAdminUsername, DefaultAdminPassword, true));
                _smallPostersContext.SaveChanges();
            }
        }
        [HttpPost("register")]
        public ActionResult<UserIdentifiedDTO> Register([FromBody]UserRegisterDTO userRegisterDTO)
        {
            string username = userRegisterDTO.Username;
            string password = userRegisterDTO.Password;
            if (_smallPostersContext.Accounts.Any(a => a.Username == username))
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }
            Models.Account accountToAdd = new Models.Account(username, password, false);
            _smallPostersContext.Accounts.Add(accountToAdd);

            _smallPostersContext.SaveChanges();
            string authTokenString = HashPair.Generate(TokenGenerator.GetUniqueKey(TokenSize), accountToAdd.Salt);
            string hashedAuthTokenString = HashPair.Generate(authTokenString, accountToAdd.Salt);
            Models.AuthToken authToken = new Models.AuthToken(hashedAuthTokenString,accountToAdd);
             _smallPostersContext.AuthTokens.Add(authToken);
            _smallPostersContext.SaveChanges();
            return new UserIdentifiedDTO { Username = username,AuthToken=authTokenString, IsAdmin = accountToAdd.IsAdmin };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserIdentifiedDTO>> Login([FromBody]UserLoginDTO userRegisterDTO)
        {
            string username = userRegisterDTO.Username;
            string password = userRegisterDTO.Password;
            Models.Account accountToFind = _smallPostersContext.Accounts.FirstOrDefault(a => a.Username == username);
            if (accountToFind==null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            string passwordHash=HashPair.Generate(password, accountToFind.Salt);
            if (accountToFind.PasswordHash == passwordHash)
            {
                string authTokenString = HashPair.Generate(TokenGenerator.GetUniqueKey(TokenSize), accountToFind.Salt);
                string hashedAuthTokenString = HashPair.Generate(authTokenString, accountToFind.Salt);
                Models.AuthToken authToken = new Models.AuthToken(hashedAuthTokenString,accountToFind);
                _smallPostersContext.AuthTokens.Add(authToken);
                await _smallPostersContext.SaveChangesAsync();
                return new UserIdentifiedDTO { Username = username, AuthToken = authTokenString,IsAdmin=accountToFind.IsAdmin };
            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
        }
        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody]UserIdentifiedDTO userIdentifiedDTO)
        {
            string username = userIdentifiedDTO.Username;
            string authTokenString = userIdentifiedDTO.AuthToken;
            Account accountToFind = _smallPostersContext.Accounts.FirstOrDefault(a => a.Username == username);
            if (accountToFind == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            string hashedAuthTokenString = HashPair.Generate(authTokenString, accountToFind.Salt);
            AuthToken authToken = _smallPostersContext.AuthTokens.FirstOrDefault
                (a => a.HashedValue == hashedAuthTokenString && a.AccountId == accountToFind.Id);
            if (authToken==null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            else
            {
                authToken.IsValid = false;
                await _smallPostersContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK);
            }
        }
    }
}
