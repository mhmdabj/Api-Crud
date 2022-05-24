namespace ApiCrud.Dto
{
    public class UserForCreationDto
    {
        public string? Name { get; set; } = string.Empty;
        public string? Username { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Password_Hash { get; set; } = string.Empty;
        public int GroupId { get; set; }
    }
    public class UserForGroupDto
    {
        public int GroupId { get; set; }
    }

    public class UserForCreationDtoGoogle
    {
        public string? Name { get; set; } = string.Empty;
        public string? Username { get; set; } = string.Empty;
        public string? Password_Hash { get; set; } = string.Empty;
    }
    public class UserForRegisterDto
    {
        public string? Name { get; set; } = string.Empty;
        public string? Username { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Password_Hash { get; set; } = string.Empty;
    }
}
