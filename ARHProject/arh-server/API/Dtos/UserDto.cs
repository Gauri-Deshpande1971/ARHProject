using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class UserDto
    {
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string Token { get; set; }

        public string AppRoleCode { get; set; }
        public string AppRoleName { get; set; }

        public string OtpCode { get; set; }

        public bool ChangePassword { get; set; }

        public string HandsetCode { get; set; }

    }
}
