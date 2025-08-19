using System;
using Game.Logic.Command.Commands.Base;

namespace Game.Logic.Command
{
    public class CommandHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public void Handle(BaseCommand command)
        {
            var commandType = command.GetType();
            foreach (var prop in commandType.GetProperties())
            {
                if (prop.CanWrite && _serviceProvider.GetService(prop.PropertyType) != null)
                {
                    var service = _serviceProvider.GetService(prop.PropertyType);
                    prop.SetValue(command, service);
                }
            }

            command.Execute();
        }
    }
}