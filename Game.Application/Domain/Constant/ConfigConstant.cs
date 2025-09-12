using System;

namespace Game.Application.Domain.Constant
{
    public static class ConfigConstant
{
    public static readonly string Url;

    static ConfigConstant()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        Url = env == "Development" ? "http://localhost:3000" : "https://mydomain.com/api";
    }
}
}