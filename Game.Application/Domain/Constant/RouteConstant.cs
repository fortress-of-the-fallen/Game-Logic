namespace Game.Application.Domain.Constant
{
    public static class RouteConstant
    {
        public struct Auth
        {
            public const string Base = "/v1/auth";
            public const string Login = Base + "/login";
            public const string Register = Base + "/register";
            public const string GithubLogin = Base + "/github-login";
            public const string Logout = Base + "/logout";
        }
    }
}
