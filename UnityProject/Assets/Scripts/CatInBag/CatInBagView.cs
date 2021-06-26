using Assets.Scripts.Data;
using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class CatInBagView : ViewBase
    {
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private CatInBagSystem CatInBagSystem { get; set; }
        
        private CatInBagPlayState PlayState => PlayStateData.PlayState as CatInBagPlayState;
        
        public SoundEffect MeowIntro;
        public SoundEffect MeowAngry;

        public Button FinishButton;
        
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
            MetagameEvents.PlayerBoardWidgetClicked.Subscribe(OnPlayerBoardWidgetClicked);
            //     CatInBagData.MeowAngry.Play();
        }
        
        protected override void OnShown()
        {
            RefreshUI();
            MeowIntro.Play();
        }

        private void RefreshUI()
        {
            FinishButton.gameObject.SetActive(NetworkData.IsMaster);
            FinishButton.interactable = PlayState.WasGiven;
            
            string currentPlayerName = PlayersBoardSystem.GetCurrentPlayerName();

            CatInBagInfo catInBagInfo = PlayState.NetQuestion.CatInBagInfo;
            
            SelectOwnerState.SetActive(!PlayState.WasGiven);
            WhoSelectPlayerName.text = currentPlayerName;
            CanGiveYourselfState.SetActive(catInBagInfo.CanGiveYourself);
            CantGiveYourselfState.SetActive(!catInBagInfo.CanGiveYourself);
            
            AnsweringState.SetActive(PlayState.WasGiven);
            WhoAnswerPlayerName.text = currentPlayerName;
            Theme.text = catInBagInfo.Theme;
            Price.text = catInBagInfo.Price.ToString();
        }

        private void OnPlayerBoardWidgetClicked(PlayerData playerData)
        {
            if (IsActive)
                CatInBagSystem.Give(playerData);
        }

        public void OnFinishButtonClicked()
        {
            CatInBagSystem.Finish();
        }
    }
}