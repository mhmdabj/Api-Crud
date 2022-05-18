using System.Data;
using ApiCrud.Context;
using ApiCrud.Contracts;
using ApiCrud.Dto;
using ApiCrud.Entities;
using Dapper;

namespace ApiCrud.Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly DapperContext _context;

        public GroupRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Group>> GetGroup()
        {
            var query = "SELECT * FROM user_group";
            using var connection = _context.CreateConnection();
            var group = await connection.QueryAsync<Group>(query);
            return group.ToList();
        }

        public async Task<Group> GetGroup(int id)
        {
            var query = "SELECT * FROM user_group Where Id=@Id";
            using var connection = _context.CreateConnection();
            var group = await connection.QuerySingleOrDefaultAsync<Group>(query, new { id });
            return group;
        }

        public async Task<Group> CreateGroup(GroupForCreationDto group)
        {
            var query = "INSERT INTO user_group (Name, group_lvl) VALUES (@Name,@group_lvl);SELECT LAST_INSERT_ID();";
            var parameters = new DynamicParameters();
            parameters.Add("Name", group.Name, DbType.String);
            parameters.Add("Group_lvl", group.Group_lvl, DbType.Int64);
            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);
                var createdGroup = new Group
                {
                    Id = id,
                    Name = group.Name,
                    Group_lvl = group.Group_lvl
                };
                return createdGroup;
            }
        }
        public async Task UpdateGroup(int id, GroupForUpdateDto group)
        {
            var query = "UPDATE user_group SET Name = @Name WHERE Id = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            parameters.Add("Name", group.Name, DbType.String);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
        public async Task DeleteGroup(int id)
        {
            var query = "DELETE FROM user_group WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { id });
            }
        }
        public async Task<IEnumerable<GroupWithUser>> GetGroupUsersMultipleResults(int id)
        {
            var query2 = "SELECT DISTINCT user.username as UserName, user_group.name as GroupName" +
                " FROM user_group_link JOIN user on user_group_link.user_id = user.id" +
                " JOIN user_group on user_group_link.group_id = user_group.id WHERE group_id= @Id";
            using var connection = _context.CreateConnection();
            var multi = await connection.QueryAsync<GroupWithUser>(query2, new { id });
            return multi.ToList();
        }
    }
}
