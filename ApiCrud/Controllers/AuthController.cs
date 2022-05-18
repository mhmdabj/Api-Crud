using System.Data;
using ApiCrud.Context;
using ApiCrud.Dto;
using ApiCrud.Entities;
using Dapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using ApiCrud.Utils;
using static ApiCrud.Models.AccountTypeEnum;

namespace ApiCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DapperContext _context;
        private readonly IConfiguration _configuration;
        private readonly AuthUtil _authUtil;
        public AuthController(DapperContext context, IConfiguration configuration, AuthUtil authUtil)
        {
            _context = context;
            _configuration = configuration;
            _authUtil = authUtil;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserForRegisterDto request)
        {
            var AccountType = AccountPermissions.Default_Type;
            _authUtil.CreatePasswordHash(request.Password_Hash, out byte[] passwordHash, out byte[] passwordSalt);
            var query = "INSERT INTO user (name, username, email, password_hash, password_salt, type) VALUES (@Name, @Username,@email, @Password_Hash, @password_salt, @type);" + "SELECT LAST_INSERT_ID();";
            String passHashString = Convert.ToBase64String(passwordHash);
            String passSaltString = Convert.ToBase64String(passwordSalt);
            var parameters = new DynamicParameters();
            parameters.Add("name", request.Name, DbType.String);
            parameters.Add("username", request.Username, DbType.String);
            parameters.Add("email", request.Email, DbType.String);
            parameters.Add("password_hash", passHashString, DbType.String);
            parameters.Add("password_salt", passSaltString, DbType.String);
            parameters.Add("type", AccountType, DbType.Int64);
            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);
                var createdUser = new User
                {
                    Id = id,
                    Name = request.Name,
                    UserName = request.Username,
                    Email = request.Email,
                    PasswordHash = passHashString,
                    PasswordSalt = passSaltString,
                    Type = (int)AccountType
                };
                return createdUser;
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(LoginDto request)
        {
            var query = "SELECT * FROM user Where username=@Username";
            using var connection = _context.CreateConnection();
            var user = await connection.QuerySingleOrDefaultAsync<User>(query, new { request.Username });
            if (user == null)
            {
                return NotFound("Wrong Credentials.");
            }
            if (user.UserName != request.Username)
            {
                return Unauthorized("Wrong username");
            }
            /*if (user.accountId != string.Empty)
            {
                return Unauthorized("Google Signin Required!");
            }*/
            byte[] passwordHash = Convert.FromBase64String(user.PasswordHash);
            byte[] passwordSalt = Convert.FromBase64String(user.PasswordSalt);
            if (_authUtil.VerifyPasswordHash(request.Password, passwordHash, passwordSalt) == false)
            {
                return Unauthorized("Wrong password");
            }
            var parameters = new DynamicParameters();
            string token = _authUtil.CreateToken(user);
            parameters.Add("token", token, DbType.String);
            parameters.Add("username", request.Username, DbType.String);
            var tokenInsertion = "UPDATE user SET token=@token WHERE username=@Username";
            await connection.ExecuteAsync(tokenInsertion, parameters);
            return Ok(token);
        }
        [HttpPost("forgot-password")]
        public async Task<ActionResult<User>> ForgotPassword(ForgotPasswordDto request)
        {
            var AccountType = AccountPermissions.Default_Type;
            var parameters = new DynamicParameters();
            parameters.Add("email", request.Email, DbType.String);
            parameters.Add("accountType", AccountType, DbType.Int64);
            var query = "SELECT * FROM user Where email=@email AND type=@AccountType";
            using var connection = _context.CreateConnection();
            var user = await connection.QuerySingleOrDefaultAsync<User>(query, parameters);

            var exists = connection.ExecuteScalar<bool>("select count(1) from user where email=@Email AND type=@AccountType", parameters);
            if (!exists)
            {
                return NotFound("Email Not found!");
            }
            string token = _authUtil.CreateToken(user);
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("api@scinops.com"));
            email.To.Add(MailboxAddress.Parse(request.Email));
            email.Subject = "API token";
            email.Body = new TextPart(TextFormat.Html) { Text = $@"Copy the Token: <code>{token}</code>" };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("lina.hansen25@ethereal.email", "1Yc3U1ECDdcY1ZfHtB");
            smtp.Send(email);
            smtp.Disconnect(true);
            return Ok(new { message = "Please check your email for password reset instructions" });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("reset-password")]
        public async Task<ActionResult<User>> ResetPassword(ResetPasswordDto request)
        {
            if (request.Password == request.Repassword)
            {
                var connection = _context.CreateConnection();
                var AccountType = AccountPermissions.Default_Type;
                _authUtil.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                String passHashString = Convert.ToBase64String(passwordHash);
                String passSaltString = Convert.ToBase64String(passwordSalt);
                var parameters = new DynamicParameters();
                parameters.Add("email", request.Email, DbType.String);
                parameters.Add("password_hash", passHashString, DbType.String);
                parameters.Add("password_salt", passSaltString, DbType.String);
                parameters.Add("accountType", AccountType, DbType.Int64);
                var exists = connection.ExecuteScalar<bool>("select count(1) from user where email=@Email AND type=@AccountType", parameters);
                if (!exists)
                {
                    return NotFound("Email Not found!");
                }
                var query = "UPDATE user SET Password_hash=@password_hash, password_salt=@password_salt WHERE email = @email AND type=@AccountType";
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, parameters);
                return Ok(new { message = "Password is changed successfully!" });
            }
            else return BadRequest("Password must be identical!");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("logout")]
        public async Task<ActionResult> LogoutUser()
        {
            var claims = this.User.Claims;
            string? token=null;
            var id = claims.Where(x => x.Type == "Id").First();
            if (id.Value == null){
                return Unauthorized("You should login first!");
            }
            var parameters = new DynamicParameters();
            parameters.Add("id", id.Value, DbType.Int64);
            parameters.Add("token", token, DbType.String);
            var query = "UPDATE user SET token=@token Where id=@id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
            return Ok(new { message = "You have been logged out!" });
        }
    }
}
