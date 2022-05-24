using System.Data;
using System.Security.Cryptography;
using ApiCrud.Context;
using ApiCrud.Contracts;
using ApiCrud.Dto;
using ApiCrud.Entities;
using ApiCrud.Utils;
using Dapper;
using static ApiCrud.Models.AccountTypeEnum;

namespace ApiCrud.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;
        private readonly AuthUtil _authUtil;

        public UserRepository(DapperContext context, AuthUtil authUtil)
        {
            _context = context;
            _authUtil = authUtil;
        }
        public async Task<IEnumerable<GetUser>> GetAllUsers()
        {
            var query = "SELECT user.id as Id, user.name as Name,user.username as Username,user.email as Email, user.accountId as AccId, user.token as Token, user_group.group_lvl as group_lvl, user_group.name as group_name FROM `user`" +
                " INNER JOIN user_group_link ON user.id=user_group_link.user_id" +
                " INNER JOIN user_group ON user_group.id=user_group_link.group_id";
            using var connection = _context.CreateConnection();
            var user = await connection.QueryAsync<GetUser>(query);
            return user.ToList();
        }

        public async Task<GetUser> GetUser(int id)
        {
            var query = "SELECT Id, Name, Username, Email FROM `user` Where id=@id";
            using var connection = _context.CreateConnection();
            var user = await connection.QuerySingleOrDefaultAsync<GetUser>(query, new { id });
            System.Diagnostics.Debug.WriteLine(user);
            return user;
        }

        public async Task<GetUserInfo> GetUserInfo(int id)
        {
            var query = "SELECT user.id as Id, user.name as Name,user.username as Username,user.email as Email, user.accountId as AccId, user.token as Token, user_group.group_lvl as group_lvl, user_group.name as group_name FROM `user`" +
                " INNER JOIN user_group_link ON user.id=user_group_link.user_id" +
                " INNER JOIN user_group ON user_group.id=user_group_link.group_id" +
                " Where user.id=@id";
            using var connection = _context.CreateConnection();
            var user = await connection.QuerySingleOrDefaultAsync<GetUserInfo>(query, new { id });
            System.Diagnostics.Debug.WriteLine(user);
            return user;
        }

        public async Task<User> CreateUser(UserForCreationDto user)
        {
            var AccountType = AccountPermissions.Default_Type;
            _authUtil.CreatePasswordHash(user.Password_Hash, out byte[] passwordHash, out byte[] passwordSalt);
            String passHashString = Convert.ToBase64String(passwordHash);
            String passSaltString = Convert.ToBase64String(passwordSalt);
            var parameters = new DynamicParameters();
            parameters.Add("name", user.Name, DbType.String);
            parameters.Add("username", user.Username, DbType.String);
            parameters.Add("email", user.Email, DbType.String);
            parameters.Add("password_hash", passHashString, DbType.String);
            parameters.Add("password_salt", passSaltString, DbType.String);
            parameters.Add("accountType", AccountType, DbType.Int32);
            parameters.Add("groupId", user.GroupId, DbType.Int32);

            var query = "INSERT INTO user (name, username, email, password_hash, password_salt, type) VALUES (@Name, @UserName, @Email, @Password_Hash, @password_salt, @AccountType);" +
                "INSERT INTO user_group_link (user_id, group_id) VALUES (LAST_INSERT_ID(), @GroupId);" +
                "SELECT LAST_INSERT_ID();";
            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parameters);
                var createdUser = new User
                {
                    Name = user.Name,
                    UserName = user.Username,
                    Email = user.Email,
                    PasswordHash = passHashString,
                    PasswordSalt = passSaltString,
                    Type = (int)AccountType,
                    GroupId = user.GroupId,
                };
                return createdUser;
            }
        }
        public async Task AddUserToGroup(int id,UserForGroupDto user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            parameters.Add("groupId", user.GroupId, DbType.Int64);

            var query = "INSERT INTO user_group_link (user_id, group_id) VALUES (@Id, @groupId)";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
        public async Task UpdateUser(int id, UserForUpdateDto user)
        {
            var query = "UPDATE user,user_group_link SET user.name = @Name, user.UserName= @UserName, user.email=@email, user_group_link.group_id=@GroupId WHERE user.id = @Id AND user_group_link.user_id=@id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            parameters.Add("Name", user.Name, DbType.String);
            parameters.Add("UserName", user.UserName, DbType.String);
            parameters.Add("Email", user.Email, DbType.String);
            parameters.Add("GroupId", user.GroupId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
        public async Task DeleteUser(int id)
        {
            var queryDeleteForeign = "DELETE FROM user_group_link WHERE user_id = @Id";
            var queryDeleteUser = "DELETE FROM user  WHERE id = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(queryDeleteForeign, new { id });
                await connection.ExecuteAsync(queryDeleteUser, new { id });
            }
        }
    }
}
