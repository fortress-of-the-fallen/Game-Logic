namespace Game.Application.Domain.Constant
{
    public static class LanguageConstant
    {
        public static readonly string[] Languages = { "English", "Vietnamese" };

        public static string English => Languages[0];
        public static string Vietnamese => Languages[1];
    }
}