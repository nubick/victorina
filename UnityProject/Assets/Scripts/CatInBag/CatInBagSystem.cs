using Injection;
using Victorina.Commands;

namespace Victorina
{
    public class CatInBagSystem
    {
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
        public void Initialize()
        {
            MetagameEvents.PlayerBoardWidgetClicked.Subscribe(OnPlayerBoardWidgetClicked);
        }

        private void OnPlayerBoardWidgetClicked(PlayerData playerData)
        {
            CommandsSystem.AddNewCommand(new GiveCatInBagCommand {ReceiverPlayerId = playerData.PlayerId});
        }
    }
}