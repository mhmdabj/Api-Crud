namespace ApiCrud.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? PasswordSalt { get; set; }
        public int GroupId { get; set; }
        public int Type { get; set; }
        public DateTime Creation_date { get; set; } = DateTime.Now;
    }
    public class GoogleFbUser
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? accountId { get; set; }
        public int Type { get; set; }
        public DateTime Creation_date { get; set; } = DateTime.Now;
    }
    public class GetUser
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public int AccId { get; set; }
        public string? Token { get; set; }
        public int Group_lvl { get; set; }
        public string? Group_name { get; set; }
    }
}
