using System.ComponentModel.DataAnnotations;

namespace KavifxApp.Models.DTO
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserDTO
    {        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class UserRoleDTO
    {
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int UserId { get; set; }
    }
}
