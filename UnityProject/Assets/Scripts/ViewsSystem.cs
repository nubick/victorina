using System;
using Injection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Victorina
{
    public class ViewsSystem
    {
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private LobbyView LobbyView { get; set; }
        [Inject] private RoundView RoundView { get; set; }
        [Inject] private FinalRoundView FinalRoundView { get; set; }
        [Inject] private PlayersBoardView PlayersBoardView { get; set; }
        [Inject] private TextStoryDotView TextStoryDotView { get; set; }
        [Inject] private ImageStoryDotView ImageStoryDotView { get; set; }
        [Inject] private AudioStoryDotView AudioStoryDotView { get; set; }
        [Inject] private VideoStoryDotView VideoStoryDotView { get; set; }
        [Inject] private NoRiskStoryDotView NoRiskStoryDotView { get; set; }
        [Inject] private CatInBagView CatInBagView { get; set; }
        [Inject] private AuctionView AuctionView { get; set; }
        [Inject] private PlayersMoreInfoView PlayersMoreInfoView { get; set; }

        [Inject] private MasterQuestionPanelView MasterQuestionPanelView { get; set; }
        [Inject] private MasterAcceptAnswerView MasterAcceptAnswerView { get; set; }

        [Inject] private PlayerGiveAnswerView PlayerGiveAnswerView { get; set; }
        
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PackagePlayStateData PlayStateData { get; set; }

        public void Initialize()
        {
            foreach(ViewBase view in Object.FindObjectsOfType<ViewBase>())
                view.Content.SetActive(false);
            StartupView.Show();
            
            MetagameEvents.PackagePlayStateChanged.Subscribe(OnPackagePlayStateChanged);
            MetagameEvents.DisconnectedAsClient.Subscribe(ShowStartUpView);
            MetagameEvents.ServerStopped.Subscribe(ShowStartUpView);
        }
        
        private void HideAll()
        {
            foreach(ViewBase view in Object.FindObjectsOfType<ViewBase>())
                if (view.IsActive)
                {
                    if(view is IDataDependOnlyView)
                        continue;
                    
                    view.Hide();
                }
        }

        private void OnPackagePlayStateChanged()
        {
            Debug.Log($"Views. OnPackagePlayState changed: {PlayStateData}");

            switch (PlayStateData.Type)
            {
                case PlayStateType.Lobby:
                    ShowLobbyViews();
                    break;
                case PlayStateType.FinalRound:
                    ShowFinalRoundViews();
                    break;
                case PlayStateType.Round:
                case PlayStateType.RoundBlinking:
                    ShowSimpleRoundViews();
                    break;
                case PlayStateType.Auction:
                    ShowAuction();
                    break;
                case PlayStateType.CatInBag:
                    ShowCatInBagViews();
                    break;
                case PlayStateType.ShowQuestion:
                case PlayStateType.ShowAnswer:
                    ShowStoryDotView();
                    break;
                default:
                    throw new Exception($"Not supported PackagePlayState: {PlayStateData.PlayState}");
            }
        }

        public void StartAnswering()
        {
            //RoundView.StartCoroutine(SwitchToQuestionView(MatchData.SelectedRoundQuestion, QuestionAnswerData));
        }
        
        // private IEnumerator SwitchToQuestionView(NetRoundQuestion netRoundQuestion, QuestionAnswerData data)
        // {
        //     if (data.QuestionType == QuestionType.Auction)
        //         ShowAuction();
        //     else
        //         UpdateStoryDot(data);
        // }

        private void ShowLobbyViews()
        {
            HideAll();
            Debug.Log("Show: LobbyView");
            LobbyView.Show();
            PlayersBoardView.Show();
        }

        private void ShowSimpleRoundViews()
        {
            HideAll();
            Debug.Log("Show: RoundView");
            RoundView.Show();
            PlayersBoardView.Show();
        }

        private void ShowFinalRoundViews()
        {
            HideAll();
            Debug.Log("Show: FinalRoundView");
            FinalRoundView.Show();
            PlayersBoardView.Show();
        }

        private void ShowCatInBagViews()
        {
            HideAll();
            Debug.Log("Show: CatInBag");
            CatInBagView.Show();

            if (!PlayStateData.As<CatInBagPlayState>().WasGiven)
                PlayersBoardView.Show();
        }

        private void ShowStoryDotView()
        {
            HideAll();
        }
        
        public void UpdateStoryDot(QuestionAnswerData data)
        {
            HideAll();

            //todo: finish refactoring
            StoryDot currentStoryDot = null;
            
            if (currentStoryDot is ImageStoryDot)
            {
                Debug.Log("Show: ImageStoryDotView");
                ImageStoryDotView.Show();

                if (NetworkData.IsMaster)
                    MasterQuestionPanelView.Show();

                if (NetworkData.IsClient)
                    PlayerGiveAnswerView.Show();
            }
            else if (currentStoryDot is AudioStoryDot)
            {
                Debug.Log("Show: AudioStoryDotView");
                AudioStoryDotView.Show();

                if (NetworkData.IsMaster)
                    MasterQuestionPanelView.Show();

                if (NetworkData.IsClient)
                    PlayerGiveAnswerView.Show();
            }
            else if (currentStoryDot is VideoStoryDot)
            {
                Debug.Log("Show: VideoStoryDotView");
                VideoStoryDotView.Show();

                if (NetworkData.IsMaster)
                    MasterQuestionPanelView.Show();

                if (NetworkData.IsClient)
                    PlayerGiveAnswerView.Show();
            }
            // else if (currentStoryDot is NoRiskStoryDot)
            // {
            //     Debug.Log("Show: NoRiskStoryDotView");
            //     NoRiskStoryDotView.Show();
            //
            //     if (NetworkData.IsMaster)
            //         MasterQuestionPanelView.Show();
            // }
            else if (currentStoryDot is TextStoryDot)
            {
                Debug.Log("Show: TextStoryDotView");
                TextStoryDotView.Show();

                if (NetworkData.IsMaster)
                    MasterQuestionPanelView.Show();

                if (NetworkData.IsClient)
                    PlayerGiveAnswerView.Show();
            }
            else
            {
                throw new Exception($"Not supported story dot: {currentStoryDot}");
            }

            //MasterEffectsView.Show();
        }

        private void ShowAuction()
        {
            Debug.Log("Show: AuctionView");
            AuctionView.Show();
            PlayersBoardView.Show();
            PlayersMoreInfoView.Show();
        }
        
        private void OnQuestionAnswerPhaseChanged()
        {
            //todo: finish refactoring
            /*
            if (QuestionAnswerData.Phase.Value == QuestionPhase.AcceptingAnswer)
            {
                if (NetworkData.IsMaster)
                    MasterAcceptAnswerView.Show();
            }
            else
            {
                if(MasterAcceptAnswerView.IsActive)
                    MasterAcceptAnswerView.Hide();
            }
            */
        }
        
        private void ShowStartUpView()
        {
            HideAll();
            StartupView.Show();
        }
    }
}