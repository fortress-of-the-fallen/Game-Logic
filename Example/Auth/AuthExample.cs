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
    }
}