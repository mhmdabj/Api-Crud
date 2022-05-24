using ApiCrud.Contracts;
using ApiCrud.Dto;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCrud.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/permission")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionRepository _permissionRepo;
        private readonly IGroupRepository _groupRepo;
        public PermissionController(IPermissionRepository permissionRepo, IGroupRepository groupRepo)
        {
            _permissionRepo = permissionRepo;
            _groupRepo = groupRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPermissions()
        {
            try
            {
                var permissions = await _permissionRepo.GetAllPermissions();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("{id}", Name = "PermissionById")]
        public async Task<IActionResult> GetPermission(int id)
        {
            try
            {
                var permission = await _permissionRepo.GetPermission(id);
                if (permission == null)
                    return NotFound();
                return Ok(permission);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreatePermission(PermissionForCreationDto permission)
        {
            try
            {
                var groupExists = _groupRepo.GetGroupBylvl(permission.GroupLevel);
                var fieldExists = _permissionRepo.GetActionField(permission.ActionField);
                if (!groupExists)
                    return StatusCode(404, "Group doesn't exists.");
                if (!fieldExists)
                    return StatusCode(404, "Field doesn't exists");
                var createdPermission = await _permissionRepo.CreatePermission(permission);
                return CreatedAtRoute("PermissionById", new { id = createdPermission.Id }, createdPermission);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, PermissionForUpdateDto permission)
        {
            try
            {
                var groupExists = _groupRepo.GetGroupBylvl(permission.GroupLevel);
                var fieldExists = _permissionRepo.GetActionField(permission.ActionField);
                if (!groupExists)
                    return StatusCode(404, "Group doesn't exists.");
                if (!fieldExists)
                    return StatusCode(404, "Field doesn't exists");
                var dbPermission = await _permissionRepo.GetPermission(id);
                if (dbPermission == null)
                    return NotFound();
                await _permissionRepo.UpdatePermission(id, permission);
                return StatusCode(201, "Updated successfully");
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            try
            {
                var dbPermission = await _permissionRepo.GetPermission(id);
                if (dbPermission == null)
                    return NotFound();
                await _permissionRepo.DeletePermission(id);
                return StatusCode(200, "Deleted successfully");
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
    }
}
