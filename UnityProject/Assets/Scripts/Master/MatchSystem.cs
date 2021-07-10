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
        [Inject] private PlayStateSystem PlayStateSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private PackageData PackageData { get; set; }
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        
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

            CommandsSystem.AddNewCommand(new SelectRoundCommand(number));
        }

        public void NavigateToRound(int number)
        {
            MatchData.RoundNumber = number;
            Round round = PackageData.Package.Rounds[number - 1];
            if (round.Type == RoundType.Simple)
            {
                PlayStateSystem.ChangeToRoundPlayState(number);
            }
            else if (round.Type == RoundType.Final)
            {
                PlayStateSystem.ChangePlayState(new FinalRoundPlayState {Round = round});
                FinalRoundSystem.Reset();
            }
        }

        public void NavigateToNextRound()
        {
            int nextRoundNumber = MatchData.RoundNumber + 1;
            bool hasNextRound = PackageData.Package.Rounds.Count >= nextRoundNumber;
            if (hasNextRound)
            {
                NavigateToRound(nextRoundNumber);
            }
            else
            {
                ResultPlayState resultPlayState = new ResultPlayState();
                PlayStateSystem.ChangePlayState(resultPlayState);
            }
        }
    }
}