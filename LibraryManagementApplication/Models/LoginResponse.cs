namespace LibraryManagementMVC.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }
    }
}