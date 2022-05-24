using ApiCrud.Dto;
using ApiCrud.Entities;

namespace ApiCrud.Contracts
{
    public interface IPermissionRepository
    {
        public Task<IEnumerable<GroupPermissionView>> GetAllPermissions();
        public Task<GroupPermissionView> GetPermission(int id);
        public Task<GroupPermission> CreatePermission(PermissionForCreationDto permission);
        public Task UpdatePermission(int id, PermissionForUpdateDto permission);
        public Task DeletePermission(int id);
        public bool GetPermissionbyUser(int id, int action);
        public bool GetActionField(int name);
    }
}
