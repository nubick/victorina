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
        [Inject] private NoRiskView NoRiskView { get; set; }
        [Inject] private CatInBagView CatInBagView { get; set; }
        [Inject] private AuctionView AuctionView { get; set; }
        [Inject] private PlayersMoreInfoView PlayersMoreInfoView { get; set; }

        [Inject] private MasterShowQuestionView MasterShowQuestionView { get; set; }
        [Inject] private MasterAcceptAnswerView MasterAcceptAnswerView { get; set; }

        [Inject] private PlayerGiveAnswerView PlayerGiveAnswerView { get; set; }
        [Inject] private PlayerLookAnswerView PlayerLookAnswerView { get; set; }
        
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
                    HideAll();
                    Debug.Log("Show: AuctionView");
                    AuctionView.Show();
                    PlayersBoardView.Show();
                    PlayersMoreInfoView.Show();
                    break;
                case PlayStateType.CatInBag:
                    ShowCatInBagViews();
                    break;
                case PlayStateType.NoRisk:
                    HideAll();
                    Debug.Log("Show: NoRiskView");
                    NoRiskView.Show();
                    break;
                case PlayStateType.ShowQuestion:
                    ShowQuestionStoryDotView(PlayStateData.As<ShowQuestionPlayState>());
                    break;
                case PlayStateType.AcceptingAnswer:
                    ShowAcceptingAnswerViews(PlayStateData.As<AcceptingAnswerPlayState>());
                    break;
                case PlayStateType.ShowAnswer:
                    ShowAnswerStoryDotView(PlayStateData.As<ShowAnswerPlayState>());
                    break;
                default:
                    throw new Exception($"Not supported PackagePlayState: {PlayStateData.PlayState}");
            }
        }

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

        private void ShowQuestionStoryDotView(ShowQuestionPlayState showQuestionPlayState)
        {
            HideAll();
            StoryDot currentStoryDot = showQuestionPlayState.CurrentStoryDot;
            ViewBase storyDotView = GetStoryDotView(currentStoryDot);
            Debug.Log($"Show: {storyDotView.name}");
            storyDotView.Show();

            if (NetworkData.IsMaster)
                MasterShowQuestionView.Show();

            if (NetworkData.IsClient)
                PlayerGiveAnswerView.Show();
        }

        private void ShowAnswerStoryDotView(ShowAnswerPlayState showAnswerPlayState)
        {
            HideAll();
            StoryDot currentStoryDot = showAnswerPlayState.CurrentStoryDot;
            ViewBase storyDotView = GetStoryDotView(currentStoryDot);
            Debug.Log($"Show: {storyDotView.name}");
            storyDotView.Show();

            if (NetworkData.IsMaster)
                MasterShowQuestionView.Show();

            if (NetworkData.IsClient)
                PlayerLookAnswerView.Show();
        }
        
        private ViewBase GetStoryDotView(StoryDot storyDot)
        {
            return storyDot switch
            {
                ImageStoryDot _ => ImageStoryDotView,
                AudioStoryDot _ => AudioStoryDotView,
                VideoStoryDot _ => VideoStoryDotView,
                TextStoryDot _ => TextStoryDotView,
                _ => throw new Exception($"Not supported story dot: {storyDot}")
            };
        }

        private void ShowAcceptingAnswerViews(AcceptingAnswerPlayState playState)
        {
            Debug.Log("Show: Accepting answer views");
            
            ShowQuestionStoryDotView(playState.ShowQuestionPlayState);
            
            if (NetworkData.IsMaster)
                MasterAcceptAnswerView.Show();
        }
        
        private void ShowStartUpView()
        {
            HideAll();
            StartupView.Show();
        }
    }
}