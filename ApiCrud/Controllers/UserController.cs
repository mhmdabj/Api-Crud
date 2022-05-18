using System.Net;
using ApiCrud.Contracts;
using ApiCrud.Dto;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ApiCrud.Models.PermissionTypeEnum;

namespace ApiCrud.Controllers
{
/*    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]*/
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IPermissionRepository _permissionRepo;
        public UserController(IUserRepository userRepo, IPermissionRepository permissionRepo)
        {
            _userRepo = userRepo;
            _permissionRepo = permissionRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var UserPermission = UserPermissions.user_add;
                var claims = this.User.Claims;
                var id = claims.Where(x => x.Type == "Id").First();
                var user = await _userRepo.GetAllUsers();
                return Ok(user);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("{id}", Name = "UserById")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userRepo.GetUser(id);
                if (user == null)
                    return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserForCreationDto user)
        {
            try
            {
                var UserPermission = (int)UserPermissions.user_add;
                var id = User.Claims.First(x => x.Type == "Id");
                var permission = _permissionRepo.GetPermissionbyUser(int.Parse(id.Value), UserPermission);
                if (permission)
                {
                    var createdUser = await _userRepo.CreateUser(user);
                    return CreatedAtRoute("UserById", new { id = createdUser.Id }, createdUser);
                }
                else return StatusCode(403, "Forbidden");
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto user)
        {
            try
            {
                var dbUser = await _userRepo.GetUser(id);
                if (dbUser == null)
                    return NotFound();
                await _userRepo.UpdateUser(id, user);
                return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var dbUser = await _userRepo.GetUser(id);
                if (dbUser == null)
                    return NotFound();
                await _userRepo.DeleteUser(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
    }
}
