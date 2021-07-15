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
        [Inject] private MasterShowFinalRoundQuestionView MasterShowFinalRoundQuestionView { get; set; }
        [Inject] private MasterAcceptAnswerView MasterAcceptAnswerView { get; set; }
        [Inject] private MasterShowAnswerView MasterShowAnswerView { get; set; }
        [Inject] private MasterMediaControlView MasterMediaControlView { get; set; }
        
        [Inject] private PlayerGiveAnswerView PlayerGiveAnswerView { get; set; }
        [Inject] private PlayerAcceptingAnswerView PlayerAcceptingAnswerView { get; set; }
        [Inject] private PlayerLookAnswerView PlayerLookAnswerView { get; set; }
        
        [Inject] private ResultView ResultView { get; set; }
        
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }

        public void Initialize()
        {
            foreach(ViewBase view in Object.FindObjectsOfType<ViewBase>())
                view.Content.SetActive(false);
            StartupView.Show();
            
            MetagameEvents.PlayStateChanged.Subscribe(OnPackagePlayStateChanged);
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
                case PlayStateType.Round:
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
                case PlayStateType.FinalRound:
                    ShowFinalRoundViews();
                    break;
                case PlayStateType.ShowFinalRoundQuestion:
                case PlayStateType.ShowFinalRoundAnswer:
                    ShowFinalRoundQuestionViews(PlayStateData.As<StoryDotPlayState>());
                    break;
                case PlayStateType.Result:
                    ShowResultViews();
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
            ShowStoryDotView(showQuestionPlayState);

            if (NetworkData.IsMaster)
            {
                MasterShowQuestionView.Show();
                showQuestionPlayState.IsCameBackFromAcceptingAnswer = false;
            }

            if (NetworkData.IsClient)
                PlayerGiveAnswerView.Show();
        }

        private void ShowAnswerStoryDotView(ShowAnswerPlayState showAnswerPlayState)
        {
            HideAll();
            ShowStoryDotView(showAnswerPlayState);

            if (NetworkData.IsMaster)
                MasterShowAnswerView.Show();

            if (NetworkData.IsClient)
                PlayerLookAnswerView.Show();
        }

        private void ShowStoryDotView(StoryDotPlayState storyDotPlayState)
        {
            StoryDot currentStoryDot = storyDotPlayState.CurrentStoryDot;
            ViewBase storyDotView = GetStoryDotView(currentStoryDot);
            Debug.Log($"Show: {storyDotView.name}");
            storyDotView.Show();

            if (NetworkData.IsMaster && storyDotPlayState.IsMediaStoryDot)
                MasterMediaControlView.Show();
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
            
            HideAll();
            StoryDot currentStoryDot = playState.ShowQuestionPlayState.CurrentStoryDot;
            ViewBase storyDotView = GetStoryDotView(currentStoryDot);
            Debug.Log($"Show: {storyDotView.name}");
            storyDotView.Show();
            
            if (NetworkData.IsMaster)
                MasterAcceptAnswerView.Show();
            
            if (NetworkData.IsClient)
                PlayerAcceptingAnswerView.Show();
        }
        
        private void ShowStartUpView()
        {
            HideAll();
            StartupView.Show();
        }

        private void ShowFinalRoundViews()
        {
            HideAll();
            Debug.Log("Show: FinalRoundView");
            FinalRoundView.Show();
            PlayersBoardView.Show();
        }

        private void ShowFinalRoundQuestionViews(StoryDotPlayState playState)
        {
            HideAll();
            ShowStoryDotView(playState);

            if (NetworkData.IsMaster)
                MasterShowFinalRoundQuestionView.Show();
        }

        private void ShowResultViews()
        {
            HideAll();
            ResultView.Show();
            PlayersBoardView.Show();
        }
    }
}