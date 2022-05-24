using ApiCrud.Dto;
using ApiCrud.Entities;

namespace ApiCrud.Contracts
{
    public interface IUserRepository
    {
        public Task<IEnumerable<GetUser>> GetAllUsers();
        public Task<GetUser> GetUser(int id);
        public Task<GetUserInfo> GetUserInfo(int id);
        public Task<User> CreateUser(UserForCreationDto user);
        public Task UpdateUser(int id, UserForUpdateDto user);
        public Task DeleteUser(int id);
        public Task AddUserToGroup(int id, UserForGroupDto user);
    }
}
