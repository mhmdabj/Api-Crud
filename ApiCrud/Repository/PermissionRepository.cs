using System.Data;
using ApiCrud.Context;
using ApiCrud.Contracts;
using ApiCrud.Dto;
using ApiCrud.Entities;
using Dapper;

namespace ApiCrud.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly DapperContext _context;

        public PermissionRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GroupPermissionView>> GetAllPermissions()
        {
            var query = "select user_group.name as GroupName, action_field.name as Action, group_permission.permission as permission FROM group_permission" +
                        " inner join action_field on group_permission.action_field = action_field.id" +
                        " inner join user_group on group_permission.group_lvl = user_group.group_lvl";
            using var connection = _context.CreateConnection();
            var permission = await connection.QueryAsync<GroupPermissionView>(query);
            return permission.ToList();
        }

        public async Task<GroupPermissionView> GetPermission(int id)
        {
            var query = "select user_group.name as GroupName, action_field.name as Action, group_permission.permission as permission FROM group_permission" +
                " inner join action_field on group_permission.action_field = action_field.id" +
                " inner join user_group on group_permission.group_lvl = user_group.group_lvl" +
                " Where group_permission.id=@Id";
            using var connection = _context.CreateConnection();
            var permission = await connection.QuerySingleOrDefaultAsync<GroupPermissionView>(query, new { id });
            return permission;
        }

        public bool GetPermissionbyUser(int id,int action)
        {
            using var connection = _context.CreateConnection();
            var query = "SELECT COUNT(1) FROM group_permission" +
                    " INNER JOIN action_field ON group_permission.action_field = action_field.id" +
                    " INNER JOIN user_group ON group_permission.group_lvl = user_group.group_lvl" +
                    " INNER JOIN user_group_link ON user_group.id = user_group_link.group_id" +
                    " WHERE user_group_link.user_id = @id AND group_permission.action_field = @action";
            var exists = connection.ExecuteScalar<bool>(query, new { id, action });
            return exists;
        }

        public async Task<GroupPermission> CreatePermission(PermissionForCreationDto permission)
        {
            var query = "INSERT INTO group_permission (group_lvl, action_field, permission) VALUES (@GroupLevel, @ActionField, @Permission);SELECT LAST_INSERT_ID();";
            var parameters = new DynamicParameters();
            parameters.Add("GroupLevel", permission.GroupLevel, DbType.Int32);
            parameters.Add("ActionField", permission.ActionField, DbType.Int32);
            parameters.Add("Permission", permission.Permission, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);
                var createdPermission = new GroupPermission
                {
                    Id = id,
                    GroupLevel = permission.GroupLevel,
                    ActionField = permission.ActionField,
                    Permission = permission.Permission,
                };
                return createdPermission;
            }
        }

        public async Task UpdatePermission(int id, PermissionForUpdateDto permission)
        {
            var query = "UPDATE group_permission SET group_lvl = @GroupLevel, action_field= @ActionField, permission=@Permission WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            parameters.Add("GroupLevel", permission.GroupLevel, DbType.Int32);
            parameters.Add("ActionField", permission.ActionField, DbType.Int32);
            parameters.Add("Permission", permission.Permission, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
        public async Task DeletePermission(int id)
        {
            var query = "DELETE FROM group_permission WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { id });
            }
        }
        public bool GetActionField(int id)
        {
            var fieldExists = "SELECT COUNT(1) FROM action_field Where id=@id";
            using var connection = _context.CreateConnection();
            var exists = connection.ExecuteScalar<bool>(fieldExists, new { id });
            return exists;
        }
    }
}
