using System.Data;
using System.Security.Claims;
using ApiCrud.Context;
using ApiCrud.Entities;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ApiCrud.Models.AccountTypeEnum;

namespace ApiCrud.Controllers
{
    [AllowAnonymous, Route("account")]
    public class GoogleFbController : Controller
    {
        private readonly DapperContext _context;
        public GoogleFbController(DapperContext context)
        {
            _context = context
;
        }
        [Route("google-login")]
        [HttpGet]
        public IActionResult GoogleSignup()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [Route("google-response")]
        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var username = result.Principal.FindFirst(ClaimTypes.Name).Value;
            var name = result.Principal.FindFirst(ClaimTypes.Surname).Value;
            var email = result.Principal.FindFirst(ClaimTypes.Email).Value;
            string none = "Null";
            var accountId = result.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            var AccountType = AccountPermissions.Google_Type;
            var parameters = new DynamicParameters();
            parameters.Add("name", name, DbType.String);
            parameters.Add("username", username, DbType.String);
            parameters.Add("email", email, DbType.String);
            parameters.Add("password_hash", none, DbType.String);
            parameters.Add("password_salt", none, DbType.String);
            parameters.Add("accountId", accountId, DbType.String);
            parameters.Add("accountType", AccountType, DbType.Int64);

            var query = "INSERT INTO user (name, username, email, password_hash, password_salt, accountId, type) VALUES (@name, @username, @email, @password_hash, @password_hash, @accountId, @AccountType);SELECT LAST_INSERT_ID();";

            using (var connection = _context.CreateConnection())
            {
                var exists = connection.ExecuteScalar<bool>("select count(1) from user where email=@email AND type=@AccountType", parameters);
                if (!exists)
                {
                    var id = await connection.QuerySingleAsync<int>(query, parameters);
                    var createdUser = new GoogleFbUser
                    {
                        Id = id,
                        Name = name,
                        UserName = username,
                        Email = email,
                        accountId = accountId,
                        Type = (int)AccountType
                    };
                    return Ok(createdUser);
                }
                else return StatusCode(409, "Google Email Already Exists!");
            }
        }
        [Route("facebook-login")]
        [HttpGet]
        public IActionResult FacebookSignup()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("FacebookResponse") };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }
        [Route("facebook-response")]
        [HttpGet]
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var username = result.Principal.FindFirst(ClaimTypes.Name).Value;
            var name = result.Principal.FindFirst(ClaimTypes.Surname).Value;
            var email = result.Principal.FindFirst(ClaimTypes.Email).Value;
            string none = "Null";
            var accountId = result.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            var AccountType = AccountPermissions.Facebook_Type;
            var parameters = new DynamicParameters();
            parameters.Add("name", name, DbType.String);
            parameters.Add("username", username, DbType.String);
            parameters.Add("email", email, DbType.String);
            parameters.Add("password_hash", none, DbType.String);
            parameters.Add("password_salt", none, DbType.String);
            parameters.Add("accountId", accountId, DbType.String);
            parameters.Add("accountType", AccountType, DbType.Int64);

            var query = "INSERT INTO user (name, username, email, password_hash, password_salt, accountId, type) VALUES (@name, @username, @email, @password_hash, @password_hash, @accountId, @AccountType);SELECT LAST_INSERT_ID();";

            using (var connection = _context.CreateConnection())
            {
                var exists = connection.ExecuteScalar<bool>("select count(1) from user where email=@email AND type=@AccountType", parameters);
                if (!exists)
                {
                    var id = await connection.QuerySingleAsync<int>(query, parameters);
                    var createdUser = new GoogleFbUser
                    {
                        Id = id,
                        Name = name,
                        UserName = username,
                        Email = email,
                        accountId = accountId,
                        Type = (int)AccountType
                    };
                    return Ok(createdUser);
                }
                else return StatusCode(409, "Facebook Email Already Exists!");
            }
        }
        [Route("logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            return SignOut();
        }
        
    }
}
