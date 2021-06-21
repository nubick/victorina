using Injection;
using UnityEngine;
using UnityEngine.UI;
using Victorina.Commands;

namespace Victorina
{
    public class NoRiskView : ViewBase
    {
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
        public Text PlayerName;
        public GameObject FinishButton;
        
        protected override void OnShown()
        {
            PlayerName.text = PlayersBoardSystem.GetCurrentPlayerName();
            FinishButton.SetActive(NetworkData.IsMaster);
        }

        public void OnFinishButtonClicked()
        {
            CommandsSystem.AddNewCommand(new FinishNoRiskCommand());
        }
    }
}