namespace EnChanger.Services.Models
{
    public class PasswordDto
    {
        public string Password { get; }

        public PasswordDto(string password)
        {
            Password = password;
        }
    }
}
