using System;
using System.Linq;
using Injection;

namespace Victorina
{
    public class MasterAnswerTipSystem
    {
        [Inject] private MasterAnswerTipData Data { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        
        private ShowQuestionPlayState PlayState => PlayStateData.As<ShowQuestionPlayState>();
        
        public void Initialize()
        {
            MetagameEvents.PlayStateChanged.Subscribe(OnPlayStateChanged);
        }

        private void OnPlayStateChanged()
        {
            if (PlayStateData.Type == PlayStateType.ShowQuestion)
            {
                Data.AnswerTip = GetAnswerTip(PlayState.NetQuestion);
                Data.IsAnswerTipEnabled = false;
            }
        }
        
        private string GetAnswerTip(NetQuestion netQuestion)
        {
            StoryDot lastStoryDot = netQuestion.AnswerStory.Last();
            if (lastStoryDot is TextStoryDot textStoryDot)
            {
                return textStoryDot.Text;
            }
            throw new Exception($"Last answer story dot is not text, {lastStoryDot}");
        }
    }
}