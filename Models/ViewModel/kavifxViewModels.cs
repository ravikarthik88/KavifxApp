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
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string OrganizationName { get; set; }
        [Required]
        public string Location { get; set; }
        [Required, DataType(DataType.Date)]
        public string DateOfBirth { get; set; }
        [Required, DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [NotMapped]
        public IFormFile ProfilePicture { get; set; }
        public byte[] ProfilePictureData { get; set; }
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
