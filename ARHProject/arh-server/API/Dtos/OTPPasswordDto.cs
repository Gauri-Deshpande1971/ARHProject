namespace API.Dtos
{
    public class OTPPasswordDto
    {
        public string UserName { get; set; }
        public string OtpCode { get; set; }
        public string OTP { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
