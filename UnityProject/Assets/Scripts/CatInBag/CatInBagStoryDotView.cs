using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    //todo: rename
    public class CatInBagStoryDotView : ViewBase
    {
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private CatInBagData CatInBagData { get; set; }
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        
        private CatInBagPlayState CatInBagPlayState => PlayStateData.PlayState as CatInBagPlayState;
        
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
            //todo: Finish refactoring
            //CatInBagData.IsPlayerSelected.SubscribeChanged(OnIsPlayerSelectedChanged);
        }

        private void OnIsPlayerSelectedChanged()
        {
            //todo: Finish refactoring
            // if (CatInBagData.IsPlayerSelected.Value)
            // {
            //     RefreshUI();
            //     CatInBagData.MeowAngry.Play();
            // }
        }
        
        protected override void OnShown()
        {
            RefreshUI();
            CatInBagData.MeowIntro.Play();
        }

        private void RefreshUI()
        {
            string currentPlayerName = PlayersBoardSystem.GetCurrentPlayerName();

            CatInBagInfo catInBagInfo = QuestionAnswerData.SelectedQuestion.Value.CatInBagInfo;
            
            SelectOwnerState.SetActive(!CatInBagPlayState.WasGiven);
            WhoSelectPlayerName.text = currentPlayerName;
            CanGiveYourselfState.SetActive(catInBagInfo.CanGiveYourself);
            CantGiveYourselfState.SetActive(!catInBagInfo.CanGiveYourself);
            
            AnsweringState.SetActive(CatInBagPlayState.WasGiven);
            WhoAnswerPlayerName.text = currentPlayerName;
            Theme.text = catInBagInfo.Theme;
            Price.text = catInBagInfo.Price.ToString();
        }
    }
}