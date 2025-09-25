using Game.Application.Base.Mediator;
using Game.Application.Service.Handlers.AuthHandler;

namespace Example.Auth
{
    public class AuthExample
    {
        public static async Task LoginExample()
        {
            var a = await Mediator.Send<LoginReq, string>(new LoginReq()
            {
                Username = "admin",
                Password = "admin123",
                RememberMe = true,
                ConnectionId = "abc"
            });
        }

        public static async Task RegisterExample()
        {
            var a = await Mediator.Send<RegisterReq, string>(new RegisterReq()
            {
                Username = "toan3210",
                Password = "toan123456",
                ConfirmPassword = "toan123456"
            });
        }

        public static async Task LogoutExample()
        {
            var a = await Mediator.Send<LogoutReq, string>(new LogoutReq()
            {
                SessionId = "hR32e_NZVR1FnMAloP8Io"
            });
        }
    }
}