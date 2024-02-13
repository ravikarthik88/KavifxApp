using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KavifxApp.Models.DTO
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required,EmailAddress]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required,Compare("Password"),DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class UserProfileViewModel 
    { 
        public int UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Organization_Name { get; set; }
        public string Location { get; set; }

        [NotMapped]
        public IFormFile ProfilePicture { get; set; }
        public string ProfilePictureUrl { get; set; }
    }

    public class RoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }

    public class PermissionViewModel
    {
        public string PermissionName { get; set; }
    }

    public class AssignRoleToUserViewModel
    {
        public string Email { get; set; }
        public string RoleName { get; set; }
    }

    public class UpdateRoleToUserViewModel
    {
        public string Email { get; set; }
        public List<string> RoleNames { get; set; }
    }

    public class RolePermissionViewModel
    {
        public string RoleName { get; set; }
        public string PermissionName { get; set; }
    }

    public class UpdateRolePermissionModel
    {
        public string RoleName { get; set; }
        public List<string> PermissionNames { get; set; }
    }

   
}
