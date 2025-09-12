using Newtonsoft.Json;

namespace Game.Application.Domain.Helpers
{
    public static class JsonHelper
    {
        public static string ToString(object obj, bool indented = false)
        {
            if (obj == null)
                return null;

            return JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None);
        }

        public static T Convert<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
