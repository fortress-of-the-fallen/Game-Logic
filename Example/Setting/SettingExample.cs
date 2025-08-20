using Game.Application.Base.Mediator;
using Game.Application.Domain.Constant;
using Game.Application.Service.Handlers.SettingHandler;
using Game.Application.Service.Models.Res.Base;
using Game.Application.Service.Models.Res.Settings;
using Newtonsoft.Json;

namespace Example.Setting
{
    public static class SettingExample
    {
        public static async Task SetSettingExample()
        {
            var a = await Mediator.Send<SetSettingReq, string>(new SetSettingReq()
            {
                Key = SettingConstant.Language,
                Value = LanguageConstant.English
            });

            Console.WriteLine(JsonConvert.SerializeObject(a));
        }

        public static async Task<ResultRes<IEnumerable<SettingsRes>>> GetSettingsExample()
        {
            var a = await Mediator.Send<GetAllSettingReq, IEnumerable<SettingsRes>>(new GetAllSettingReq());

            Console.WriteLine(JsonConvert.SerializeObject(a));
            return a;
        }
    }
}