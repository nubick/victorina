using System;
using System.Collections;
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
        [Inject] private CatInBagStoryDotView CatInBagStoryDotView { get; set; }
        [Inject] private AuctionView AuctionView { get; set; }
        [Inject] private PlayersMoreInfoView PlayersMoreInfoView { get; set; }

        [Inject] private MasterQuestionPanelView MasterQuestionPanelView { get; set; }
        [Inject] private MasterAcceptAnswerView MasterAcceptAnswerView { get; set; }
        [Inject] private MasterEffectsView MasterEffectsView { get; set; }
        
        [Inject] private PlayerGiveAnswerView PlayerGiveAnswerView { get; set; }

        [Inject] private MatchData MatchData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private CatInBagData CatInBagData { get; set; }

        public void Initialize()
        {
            foreach(ViewBase view in Object.FindObjectsOfType<ViewBase>())
                view.Content.SetActive(false);
            StartupView.Show();

            MatchData.Phase.SubscribeChanged(OnMatchPhaseChanged);
            MatchData.RoundData.SubscribeChanged(OnRoundDataChanged);
            QuestionAnswerData.Phase.SubscribeChanged(OnQuestionAnswerPhaseChanged);
            CatInBagData.IsPlayerSelected.SubscribeChanged(OnCatInBagIsPlayerSelectedChanged);
            
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

        private void OnMatchPhaseChanged()
        {
            MatchPhase phase = MatchData.Phase.Value;
            Debug.Log($"OnPhaseChanged: {phase}");
            switch (phase)
            {
                case MatchPhase.WaitingInLobby:
                    ShowLobbyViews();
                    break;
                case MatchPhase.Round:
                    ShowRoundViews();
                    break;
                case MatchPhase.Question:
                    break;
                default:
                    throw new Exception($"Not supported phase: {MatchData.Phase}");
            }
        }
        
        public void StartAnswering()
        {
            RoundView.StartCoroutine(SwitchToQuestionView(MatchData.SelectedRoundQuestion, QuestionAnswerData));
        }
        
        private IEnumerator SwitchToQuestionView(NetRoundQuestion netRoundQuestion, QuestionAnswerData data)
        {
            yield return RoundView.ShowQuestionBlinkEffect(netRoundQuestion);

            if (data.QuestionType == QuestionType.Auction)
                ShowAuction();
            else
                UpdateStoryDot(data);
        }

        private void ShowLobbyViews()
        {
            HideAll();

            Debug.Log("Show: LobbyView");
            LobbyView.Show();
            PlayersBoardView.Show();
            
            //if (NetworkData.IsMaster)
            //    MasterEffectsView.Show();
        }

        private void ShowRoundViews()
        {
            HideAll();

            int number = MatchData.RoundsInfo.Value.CurrentRoundNumber;
            RoundType roundType = MatchData.RoundsInfo.Value.RoundTypes[number - 1];

            switch (roundType)
            {
                case RoundType.Simple:
                    Debug.Log("Show: RoundView");
                    RoundView.Show();
                    PlayersBoardView.Show();
                    break;
                case RoundType.Final:
                    Debug.Log("Show: FinalRoundView");
                    FinalRoundView.Show();
                    PlayersBoardView.Show();
                    break;
                default:
                    throw new Exception($"Not supported round type: {roundType}");
            }
            
            
            //if (NetworkData.IsMaster)
            //    MasterEffectsView.Show();
        }

        private void OnRoundDataChanged()
        {
            if (MatchData.Phase.Value == MatchPhase.Round)
            {
                RoundView.RefreshUI(MatchData.RoundData.Value);
            }
        }

        public void UpdateStoryDot(QuestionAnswerData data)
        {
            HideAll();

            if (data.CurrentStoryDot is ImageStoryDot)
            {
                Debug.Log("Show: ImageStoryDotView");
                ImageStoryDotView.Show();

                if (NetworkData.IsMaster)
                    MasterQuestionPanelView.Show();

                if (NetworkData.IsClient)
                    PlayerGiveAnswerView.Show();
            }
            else if (data.CurrentStoryDot is AudioStoryDot)
            {
                Debug.Log("Show: AudioStoryDotView");
                AudioStoryDotView.Show();

                if (NetworkData.IsMaster)
                    MasterQuestionPanelView.Show();

                if (NetworkData.IsClient)
                    PlayerGiveAnswerView.Show();
            }
            else if (data.CurrentStoryDot is VideoStoryDot)
            {
                Debug.Log("Show: VideoStoryDotView");
                VideoStoryDotView.Show();

                if (NetworkData.IsMaster)
                    MasterQuestionPanelView.Show();

                if (NetworkData.IsClient)
                    PlayerGiveAnswerView.Show();
            }
            else if (data.CurrentStoryDot is NoRiskStoryDot)
            {
                Debug.Log("Show: NoRiskStoryDotView");
                NoRiskStoryDotView.Show();

                if (NetworkData.IsMaster)
                    MasterQuestionPanelView.Show();
            }
            else if (data.CurrentStoryDot is CatInBagStoryDot)
            {
                Debug.Log("Show: CatInBagStoryDotView");
                CatInBagStoryDotView.Show();
                if (!CatInBagData.IsPlayerSelected.Value)
                    PlayersBoardView.Show();

                if (NetworkData.IsMaster)
                    MasterQuestionPanelView.Show();
            }
            else if (data.CurrentStoryDot is TextStoryDot)
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
                throw new Exception($"Not supported story dot: {data.CurrentStoryDot}");
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
        }
        
        private void OnCatInBagIsPlayerSelectedChanged()
        {
            if(CatInBagData.IsPlayerSelected.Value)
                PlayersBoardView.Hide();
        }

        private void ShowStartUpView()
        {
            HideAll();
            StartupView.Show();
        }
    }
}