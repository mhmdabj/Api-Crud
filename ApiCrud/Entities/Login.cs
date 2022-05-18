namespace ApiCrud.Entities
{
    public class Login
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? EmailAddress { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
