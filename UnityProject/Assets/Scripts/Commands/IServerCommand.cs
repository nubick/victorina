namespace Victorina.Commands
{
    public interface IServerCommand
    {
        bool CanExecuteOnServer();
        void ExecuteOnServer();
    }
}