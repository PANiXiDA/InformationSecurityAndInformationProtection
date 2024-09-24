namespace Entities
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Sault { get; set; } = string.Empty;

        public User(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}
