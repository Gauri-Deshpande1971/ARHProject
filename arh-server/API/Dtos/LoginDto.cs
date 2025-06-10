namespace API.Dtos
{
    public class LoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AppMode { get; set; }
        public string IMEINo { get; set; }
        public string Version { get; set; }
        public string HandsetCode { get; set; }
        public string OSType { get; set; }
    }

    public class LoginOtpDto
    {
        public string UserName { get; set; }
        public string OtpCode { get; set; }
        public string Otp { get; set; }
    }
}
