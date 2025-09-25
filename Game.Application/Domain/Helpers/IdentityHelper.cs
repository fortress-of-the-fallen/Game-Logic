using NanoidDotNet;

namespace Game.Application.Domain.Helpers
{
    public static class IdentityHelper
    {
        public static string GenerateNanoId()
        {
            return Nanoid.Generate();
        }
    }
}