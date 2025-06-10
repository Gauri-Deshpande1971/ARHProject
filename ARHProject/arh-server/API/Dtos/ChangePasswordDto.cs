namespace API.Dtos
{
    public class ChangePasswordDto
    {
        public string LoginId { get; set; }
        public string DisplayName { get; set; }

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

    }

}