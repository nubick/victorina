using System;
using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class PlayerButtonView : ViewBase
    {
        [Inject] private QuestionTimer QuestionTimer { get; set; }
        [Inject] private PlayerAnswerSystem PlayerAnswerSystem { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private MatchData MatchData { get; set; }

        public Image TimerStrip;
        public GameObject AnswerButton;
        public GameObject AnsweringPanel;
        public Text AnsweringText;
        public GameObject AnswerPanel;
        public GameObject WasWrongAnswerPanel;
        
        public void Initialize()
        {
            QuestionAnswerData.Phase.SubscribeChanged(RefreshUI);
        }

        protected override void OnShown()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (!IsActive)
                return;
            
            QuestionPhase phase = QuestionAnswerData.Phase.Value;
            
            AnswerPanel.SetActive(phase == QuestionPhase.ShowAnswer);
            AnswerButton.SetActive(PlayerAnswerSystem.CanSendAnswerIntention());
            
            if (QuestionAnswerData.QuestionType == QuestionType.Simple)
            {
                WasWrongAnswerPanel.SetActive(phase == QuestionPhase.ShowQuestion && PlayerAnswerSystem.WasWrongAnswer());
                AnsweringPanel.SetActive(phase == QuestionPhase.AcceptingAnswer);
                AnsweringText.text = $"Отвечает: {QuestionAnswerData.AnsweringPlayerName}";
            }
            else if (QuestionAnswerData.QuestionType == QuestionType.NoRisk)
            {
                WasWrongAnswerPanel.SetActive(false);
                AnsweringPanel.SetActive(!PlayerAnswerSystem.IsMeCurrentUser() || phase == QuestionPhase.AcceptingAnswer);
                AnsweringText.text = $"Отвечает: {MatchData.PlayersBoard.Value.Current.Name}";
            }
            else
            {
                throw new Exception($"Not supported question type: {QuestionAnswerData.QuestionType}");
            }
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