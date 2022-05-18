using ApiCrud.Contracts;
using ApiCrud.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCrud.Controllers
{
    [Route("api/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupRepository _groupRepo;
        public GroupController(IGroupRepository groupRepo)
        {
            _groupRepo = groupRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetGroup()
        {
            try
            {
                var group = await _groupRepo.GetGroup();
                return Ok(group);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("{id}", Name = "GroupById")]
        public async Task<IActionResult> GetGroup(int id)
        {
            try
            {
                var group = await _groupRepo.GetGroup(id);
                if (group == null)
                    return NotFound();
                return Ok(group);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroup(GroupForCreationDto group)
        {
            try
            {
                var createdGtroup = await _groupRepo.CreateGroup(group);
                return CreatedAtRoute("GroupById", new { id = createdGtroup.Id }, createdGtroup);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, GroupForUpdateDto group)
        {
            try
            {
                var dbGroup = await _groupRepo.GetGroup(id);
                if (dbGroup == null)
                    return NotFound();
                await _groupRepo.UpdateGroup(id, group);
                return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            try
            {
                var dbGroup = await _groupRepo.GetGroup(id);
                if (dbGroup == null)
                    return NotFound();
                await _groupRepo.DeleteGroup(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("{id}/MultipleUsers")]
        public async Task<IActionResult> GetGroupUsersMultipleResults(int id)
        {
            try
            {
                var group = await _groupRepo.GetGroupUsersMultipleResults(id);
                if (group == null)
                    return NotFound();
                return Ok(group);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
    }
}
