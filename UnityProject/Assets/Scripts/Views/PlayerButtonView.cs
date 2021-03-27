using System;
using Injection;
using UnityEngine;
using UnityEngine.Serialization;
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
        public GameObject SayAnswerButton;
        public GameObject AnsweringPanel;
        public Text AnsweringText;
        public GameObject AnswerDemonstrationPanel;
        public GameObject WasWrongAnswerPanel;
        
        public void Initialize()
        {
            QuestionAnswerData.Phase.SubscribeChanged(RefreshUI);
            QuestionAnswerData.PlayersButtonClickData.SubscribeChanged(RefreshUI);
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
            
            AnswerDemonstrationPanel.SetActive(phase == QuestionPhase.ShowAnswer);
            SayAnswerButton.SetActive(PlayerAnswerSystem.CanSendAnswerIntention());

            TimerStrip.gameObject.SetActive(QuestionAnswerData.TimerState != QuestionTimerState.NotStarted);
            
            if (QuestionAnswerData.QuestionType == QuestionType.Simple)
            {
                WasWrongAnswerPanel.SetActive(phase == QuestionPhase.ShowQuestion && PlayerAnswerSystem.WasWrongAnswer());
                AnsweringPanel.SetActive(phase == QuestionPhase.AcceptingAnswer);
                AnsweringText.text = $"Отвечает: {QuestionAnswerData.AnsweringPlayerName}";
            }
            else if (QuestionAnswerData.QuestionType == QuestionType.NoRisk ||
                     QuestionAnswerData.QuestionType == QuestionType.CatInBag)
            {
                WasWrongAnswerPanel.SetActive(false);
                AnsweringPanel.SetActive(!MatchData.IsMeCurrentPlayer || phase == QuestionPhase.AcceptingAnswer);
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