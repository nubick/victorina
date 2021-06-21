using System.Linq;
using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class PlayerGiveAnswerView : ViewBase
    {
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private PlayerAnswerSystem PlayerAnswerSystem { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }

        public Image TimerStrip;

        public GameObject WaitingState;
        public GameObject SayAnswerState;
        public GameObject WasWrongAnswerState;
        
        public GameObject AnsweringPanel;
        public Text AnsweringText;
        public GameObject AnswerDemonstrationPanel;
        

        public Text ThemeText;
        
        public void Initialize()
        {
            //todo: finish refactoring
            //QuestionAnswerData.Phase.SubscribeChanged(RefreshUI);
            MetagameEvents.PlayersButtonClickDataChanged.Subscribe(RefreshUI);
        }

        protected override void OnShown()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (!IsActive)
                return;
            
            AnswerDemonstrationPanel.SetActive(false);
            WasWrongAnswerState.SetActive(false);
            AnsweringPanel.SetActive(false);
            SayAnswerState.SetActive(false);
            WaitingState.SetActive(false);

            //todo: Finish refactoring
            //if (phase == QuestionPhase.ShowQuestion)
            {
                if (PlayerAnswerSystem.WasWrongAnswer())
                    WasWrongAnswerState.SetActive(true);
                else if (PlayerAnswerSystem.WasIntentionSent())
                    WaitingState.SetActive(true);
                else if (PlayerAnswerSystem.CanSendAnswerIntentionNow())
                    SayAnswerState.SetActive(true);
                else if (PlayerAnswerSystem.CanSendAnswerIntention())
                    WaitingState.SetActive(true);
                else
                {
                    AnsweringPanel.SetActive(true);
                    string names = string.Join(", ", QuestionAnswerData.AdmittedPlayersIds.Select(PlayersBoardSystem.GetPlayerName));
                    AnsweringText.text = $"Отвечает: {names}";
                }
            }
            //else if (phase == QuestionPhase.AcceptingAnswer)
            {
                AnsweringPanel.SetActive(true);
                AnsweringText.text = $"Отвечает: {PlayersBoardSystem.GetPlayerName(QuestionAnswerData.AnsweringPlayerId)}";
            }
            //else if (phase == QuestionPhase.ShowAnswer)
            {
                AnswerDemonstrationPanel.SetActive(true);
            }
            //else
            //{
            //    Debug.LogError($"Not supported phase: {phase}");
            //}
            
            TimerStrip.gameObject.SetActive(QuestionAnswerData.TimerState != QuestionTimerState.NotStarted);
            ThemeText.text = $"Тема: {MatchData.GetTheme()}";
        }
        
        public void OnAnswerButtonClicked()
        {
            PlayerAnswerSystem.OnAnswerButtonClicked();
        }

        public void Update()
        {
            if (IsActive)
            {
                float leftSeconds = QuestionTimer.GetLeftSecondsPercentage();
                TimerStrip.fillAmount = leftSeconds;
            }
        }
    }
}