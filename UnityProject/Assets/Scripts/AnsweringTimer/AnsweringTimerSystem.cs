using Injection;
using UnityEngine;

namespace Victorina
{
    public class AnsweringTimerSystem
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private AnsweringTimerData Data { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private MatchSettingsData MatchSettingsData { get; set; }
        [Inject] private AnsweringTimerView AnsweringTimerView { get; set; }
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        [Inject] private MasterAcceptAnswerView MasterAcceptAnswerView { get; set; }
        
        public void Initialize()
        {
            //todo: finish refactoring
            //QuestionAnswerData.Phase.SubscribeChanged(OnQuestionAnswerPhaseChanged);
        }

        private void OnQuestionAnswerPhaseChanged()
        {
            if (NetworkData.IsMaster &&
                MatchSettingsData.IsLimitAnsweringSeconds &&
                QuestionAnswerData.QuestionType == QuestionType.Simple)
            {
                //todo: finish refactoring
                //if(QuestionAnswerData.Phase.Value == QuestionPhase.AcceptingAnswer)
                {
                    Data.IsRunning = true;
                    Data.MaxSeconds = MatchSettingsData.MaxAnsweringSeconds;
                    Data.LeftSeconds = Data.MaxSeconds;
                    SendToPlayersService.SendAnsweringTimerData(Data);
                }
                //else
                {
                    if (Data.IsRunning)
                    {
                        Data.IsRunning = false;
                        SendToPlayersService.SendAnsweringTimerData(Data);
                    }
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
                    SendToPlayersService.SendAnsweringTimerData(Data);
                    Data.LastTimeSend = Time.time;
                }
            }

            RefreshView();
        }

        private void RefreshView()
        {
            if(Data.IsRunning && !AnsweringTimerView.IsActive)
                AnsweringTimerView.Show();
            
            if(!Data.IsRunning && AnsweringTimerView.IsActive)
                AnsweringTimerView.Hide();
            
            if(Data.IsRunning)
                AnsweringTimerView.RefreshUI(Data);
        }

        private void AcceptAnswerAsWrong()
        {
            QuestionAnswerSystem.AcceptAnswerAsWrong();
        }
    }
}