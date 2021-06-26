namespace Victorina.Commands
{
    public interface IPlayerCommand : INetworkCommand
    {
        void ExecuteOnClient();
    }
}