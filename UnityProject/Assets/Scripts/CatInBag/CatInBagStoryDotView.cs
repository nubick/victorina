using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class CatInBagStoryDotView : ViewBase
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private CatInBagData CatInBagData { get; set; }
        
        [Header("Select owner")]
        public GameObject SelectOwnerState;
        public Text WhoSelectPlayerName;
        public GameObject CanGiveYourselfState;
        public GameObject CantGiveYourselfState;

        [Header("Answering")] 
        public GameObject AnsweringState;
        public Text WhoAnswerPlayerName;
        public Text Theme;
        public Text Price;

        public void Initialize()
        {
            CatInBagData.IsPlayerSelected.SubscribeChanged(OnIsPlayerSelectedChanged);
        }

        private void OnIsPlayerSelectedChanged()
        {
            if (CatInBagData.IsPlayerSelected.Value)
            {
                RefreshUI();
                CatInBagData.MeowAngry.Play();
            }
        }
        
        protected override void OnShown()
        {
            RefreshUI();
            CatInBagData.MeowIntro.Play();
        }

        private void RefreshUI()
        {
            string currentPlayerName = MatchData.GetCurrentPlayerName();

            CatInBagStoryDot storyDot = QuestionAnswerData.CurrentStoryDot as CatInBagStoryDot;
            
            SelectOwnerState.SetActive(!CatInBagData.IsPlayerSelected.Value);
            WhoSelectPlayerName.text = currentPlayerName;
            CanGiveYourselfState.SetActive(storyDot.CanGiveYourself);
            CantGiveYourselfState.SetActive(!storyDot.CanGiveYourself);
            
            AnsweringState.SetActive(CatInBagData.IsPlayerSelected.Value);
            WhoAnswerPlayerName.text = currentPlayerName;
            Theme.text = storyDot.Theme;
            Price.text = storyDot.Price.ToString();
        }
    }
}