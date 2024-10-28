namespace SecretSantaAPI.Controllers
{
    public interface ILoggerAPI
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message);
    }
}
