using Injection;
using Victorina.Commands;

namespace Victorina
{
    public class CatInBagSystem
    {
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
        public void Give(PlayerData playerData)
        {
            CommandsSystem.AddNewCommand(new GiveCatInBagCommand {ReceiverPlayerId = playerData.PlayerId});    
        }
        
        public void Finish()
        {
            CommandsSystem.AddNewCommand(new FinishCatInBagCommand());
        }
    }
}