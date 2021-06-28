using Injection;
using UnityEngine;

namespace Victorina
{
    public class AcceptingAnswerTimerSystem
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private AcceptingAnswerTimerData Data { get; set; }
        [Inject] private MatchSettingsData MatchSettingsData { get; set; }
        [Inject] private AcceptingAnswerTimerView AcceptingAnswerTimerView { get; set; }
        [Inject] private AcceptAnswerSystem AcceptAnswerSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }

        private AcceptingAnswerPlayState PlayState => PlayStateData.As<AcceptingAnswerPlayState>();
        
        public void Initialize()
        {
            MetagameEvents.PlayStateChanged.Subscribe(OnPlayStateChanged);
        }

        private void OnPlayStateChanged()
        {
            if (PlayStateData.Type == PlayStateType.AcceptingAnswer)
            {
                if (NetworkData.IsMaster && MatchSettingsData.IsLimitAnsweringSeconds &&
                    PlayState.ShowQuestionPlayState.NetQuestion.Type == QuestionType.Simple)
                {
                    Data.IsRunning = true;
                    Data.MaxSeconds = MatchSettingsData.MaxAnsweringSeconds;
                    Data.LeftSeconds = Data.MaxSeconds;
                    SendToPlayersService.SendAcceptingAnswerTimerData(Data);
                }
            }
            else
            {
                if (Data.IsRunning)
                {
                    Data.IsRunning = false;
                    SendToPlayersService.SendAcceptingAnswerTimerData(Data);
                }
            }
        }

        public void OnUpdate()
        {
            if (NetworkData.IsMaster && Data.IsRunning)
            {
                Data.LeftSeconds = Mathf.Max(0f, Data.LeftSeconds - Time.deltaTime);
                bool isTimeUp = Mathf.Approximately(Data.LeftSeconds, 0f);
                
                if (isTimeUp)
                {
                    Data.IsRunning = false;
                    AcceptAnswerAsWrong();
                }

                bool isTimeToSend = isTimeUp || Time.time - Data.LastTimeSend > 0.1f;
                if (isTimeToSend)
                {
                    SendToPlayersService.SendAcceptingAnswerTimerData(Data);
                    Data.LastTimeSend = Time.time;
                }
            }

            RefreshView();
        }

        private void RefreshView()
        {
            if(Data.IsRunning && !AcceptingAnswerTimerView.IsActive)
                AcceptingAnswerTimerView.Show();
            
            if(!Data.IsRunning && AcceptingAnswerTimerView.IsActive)
                AcceptingAnswerTimerView.Hide();
            
            if(Data.IsRunning)
                AcceptingAnswerTimerView.RefreshUI(Data);
        }

        private void AcceptAnswerAsWrong()
        {
            AcceptAnswerSystem.AcceptAnswerAsWrong();
        }
    }
}