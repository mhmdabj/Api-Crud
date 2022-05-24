using ApiCrud.Dto;
using ApiCrud.Entities;

namespace ApiCrud.Contracts
{
    public interface IGroupRepository
    {
        public Task<IEnumerable<Group>> GetGroup();
        public Task<Group> GetGroup(int id);
        public Task<Group> CreateGroup(GroupForCreationDto group);
        public Task UpdateGroup(int id, GroupForUpdateDto group);
        public Task DeleteGroup(int id);
        public Task<IEnumerable<GroupWithUser>> GetGroupUsersMultipleResults(int id);
        public bool GetGroupBylvl(int field);
    }
}
