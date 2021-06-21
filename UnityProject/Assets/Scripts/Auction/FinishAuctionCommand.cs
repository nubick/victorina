using Victorina.Commands;

namespace Victorina
{
    public class FinishAuctionCommand : Command, IServerCommand
    {
        public override CommandType Type => CommandType.FinishAuctionCommand;

        public bool CanExecuteOnServer()
        {
            throw new System.NotImplementedException();
        }

        public void ExecuteOnServer()
        {
            throw new System.NotImplementedException();
        }
    }
}