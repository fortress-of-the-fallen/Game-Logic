namespace Game.Application.Interface
{
    public interface ILogger<T>
    {
        void LogInformation(string message);

        void LogWarning(string message);

        void LogError(string message);

        void LogError(string message, params object[] args);

        void LogInformation(string message, params object[] args);

        void LogWarning(string message, params object[] args);
    }
}