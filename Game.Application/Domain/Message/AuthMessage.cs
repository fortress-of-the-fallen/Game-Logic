namespace Game.Application.Domain.Message
{
    public class LoginReq
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginRes
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Error { get; set; }
    }
}
