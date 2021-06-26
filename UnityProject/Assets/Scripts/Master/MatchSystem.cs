using Injection;
using Victorina.Commands;

namespace Victorina
{
    public class MatchSystem
    {
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private MessageDialogueView MessageDialogueView { get; set; }

        public void Initialize()
        {
            MetagameEvents.ServerStarted.Subscribe(OnServerStarted);
        }

        private void OnServerStarted()
        {
            PlayersBoard.Clear();
        }
        
        public void StartMatch()
        {
            SelectRound(1);
        }

        public void TrySelectQuestion(NetRoundQuestion netRoundQuestion)
        {
            if (NetworkData.IsMaster && PlayersBoard.Current == null)
            {
                MessageDialogueView.Show("Текущий игрок?", "Для старта необходим текущий игрок! Выберите игрока в верхней панели и сделайте его текущим!");
                return;
            }

            CommandsSystem.AddNewCommand(new SelectRoundQuestionCommand {QuestionId = netRoundQuestion.QuestionId});
        }
        
        public void SelectRound(int number)
        {
            if (NetworkData.IsClient)
                return;

            CommandsSystem.AddNewCommand(new SelectRoundCommand {RoundNumber = number});
        }
    }
}