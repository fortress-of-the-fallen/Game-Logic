namespace Game.Application.Domain.Message
{
    public static class SettingHandlerMessage
    {
        public struct SetSettingHandler
        {
            public const string InvalidLanguage = "SettingHandlerMessage.SetSettingHandler.InvalidLanguage";
        }

        public struct GetCurrentSettingHandler
        {
            public const string SettingNotFound = "SettingHandlerMessage.GetCurrentSettingHandler.SettingNotFound";
        }
    }
}