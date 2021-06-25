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
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        [Inject] private AnswerTimerData AnswerTimerData { get; set; }

        public Image TimerStrip;

        public GameObject WaitingState;
        public GameObject SayAnswerState;
        public GameObject WasWrongAnswerState;
        
        public GameObject AnsweringPanel;
        public Text AnsweringText;

        public Text ThemeText;

        private ShowQuestionPlayState PlayState => PlayStateData.As<ShowQuestionPlayState>();
        
        public void Initialize()
        {
            MetagameEvents.PlayersButtonClickDataChanged.Subscribe(RefreshUI);
            MetagameEvents.AnswerTimerDataChanged.Subscribe(RefreshUI);
        }

        protected override void OnShown()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (!IsActive)
                return;
            
            WasWrongAnswerState.SetActive(false);
            AnsweringPanel.SetActive(false);
            SayAnswerState.SetActive(false);
            WaitingState.SetActive(false);
            
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
                string names = string.Join(", ", PlayState.AdmittedPlayersIds.Select(PlayersBoardSystem.GetPlayerName));
                AnsweringText.text = $"Отвечает: {names}";
            }
            
            TimerStrip.gameObject.SetActive(AnswerTimerData.State != QuestionTimerState.NotStarted);
            ThemeText.text = $"Тема: {PlayState.NetQuestion.Theme}";
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