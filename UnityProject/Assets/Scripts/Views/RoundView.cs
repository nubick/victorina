using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class RoundView : ViewBase
    {
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private ServerService ServerService { get; set; }
        [Inject] private ClientService ClientService { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private MatchSettingsView MatchSettingsView { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private ViewsSystem ViewsSystem { get; set; }
        
        private readonly Dictionary<string, RoundQuestionWidget> _questionWidgetsCache = new Dictionary<string, RoundQuestionWidget>();
        
        public RectTransform ThemeWidgetsRoot;
        public ThemeWidget ThemeWidgetPrefab;

        public RectTransform QuestionsRoot;
        public Transform RoundQuestionsLinePrefab;
        public RoundQuestionWidget RoundQuestionWidgetPrefab;

        public GameObject MatchSettingsButton;
        
        [Header("Rounds Info")] 
        public RectTransform RoundsInfoRoot;
        public RoundInfoWidget RoundInfoWidgetPrefab;

        private RoundPlayState PlayState => PlayStateData.As<RoundPlayState>();
        
        public void Initialize()
        {
            MetagameEvents.RoundQuestionClicked.Subscribe(OnRoundQuestionClicked);
            MetagameEvents.RoundInfoClicked.Subscribe(OnRoundInfoClicked);
            
            ServerEvents.RoundQuestionSelected.Subscribe(OnRoundQuestionSelected);
        }
        
        protected override void OnShown()
        {
            RefreshUI(PlayState.NetRound);
        }

        protected override void OnHide()
        {
            StopAllCoroutines();
        }

        private void RefreshUI(NetRound netRound)
        {
            if (IsTheSameRound(netRound))
            {
                UpdateLayout(netRound);
            }
            else
            {
                ClearLayout();
                InstantiateLayout(netRound);
            }
            
            RefreshRoundsInfo(PlayState);
            MatchSettingsButton.SetActive(NetworkData.IsMaster);
        }
        
        private bool IsTheSameRound(NetRound netRound)
        {
            NetRoundQuestion netRoundQuestion = netRound.Themes.First().Questions.First();
            return _questionWidgetsCache.ContainsKey(netRoundQuestion.QuestionId);
        }

        private void ClearLayout()
        {
            _questionWidgetsCache.Clear();
            ClearChild(ThemeWidgetsRoot);
            ClearChild(QuestionsRoot);
        }

        private void InstantiateLayout(NetRound netRound)
        {
            int columns = netRound.Themes.Max(_ => _.Questions.Count);
            
            foreach (NetRoundTheme netRoundTheme in netRound.Themes)
            {
                ThemeWidget themeWidget = Instantiate(ThemeWidgetPrefab, ThemeWidgetsRoot);
                themeWidget.Name.text = netRoundTheme.Name;
                
                Transform questionsRoot = Instantiate(RoundQuestionsLinePrefab, QuestionsRoot);
                foreach (NetRoundQuestion netRoundQuestion in netRoundTheme.Questions)
                {
                    RoundQuestionWidget widget = Instantiate(RoundQuestionWidgetPrefab, questionsRoot);
                    widget.Bind(netRoundQuestion);
                    _questionWidgetsCache.Add(netRoundQuestion.QuestionId, widget);
                }

                for (int i = netRoundTheme.Questions.Count; i < columns; i++)
                {
                    RoundQuestionWidget widget = Instantiate(RoundQuestionWidgetPrefab, questionsRoot);
                    widget.BindEmpty();
                }
            }
        }

        private void UpdateLayout(NetRound netRound)
        {
            foreach (NetRoundQuestion netRoundQuestion in netRound.Themes.SelectMany(theme => theme.Questions))
            {
                RoundQuestionWidget widget = _questionWidgetsCache[netRoundQuestion.QuestionId];
                widget.Bind(netRoundQuestion);
            }
        }
        
        private void RefreshRoundsInfo(RoundPlayState roundPlayState)
        {
            ClearChild(RoundsInfoRoot);
            for (int number = 1; number <= roundPlayState.RoundTypes.Length; number++)
            {
                RoundInfoWidget widget = Instantiate(RoundInfoWidgetPrefab, RoundsInfoRoot);
                RoundProgress roundProgress =
                    number < roundPlayState.RoundNumber ? RoundProgress.Passed :
                    number > roundPlayState.RoundNumber ? RoundProgress.Next : RoundProgress.Current;
                string title = roundPlayState.RoundNames[number - 1];
                widget.Bind(title, number, roundProgress); //passed, current, next
            }
        }

        public void OnBackButtonClicked()
        {
            if (NetworkData.IsMaster)
                ServerService.StopServer();
            else
                ClientService.LeaveGame();
            
            ViewsSystem.ShowStartUpView();
        }
        
        private void OnRoundQuestionClicked(NetRoundQuestion netRoundQuestion)
        {
            MatchSystem.TrySelectQuestion(netRoundQuestion);
        }
        
        private IEnumerator ShowQuestionBlinkEffect(string questionId)
        {
            WaitForSeconds delay = new WaitForSeconds(0.1f);
            RoundQuestionWidget cachedWidget = null;
            for (;;)
            {
                if (cachedWidget == null || cachedWidget.NetRoundQuestion.QuestionId != questionId)
                    cachedWidget = QuestionsRoot.GetComponentsInChildren<RoundQuestionWidget>().Single(_ => _.NetRoundQuestion != null && _.NetRoundQuestion.QuestionId == questionId);

                cachedWidget.ShowHighlighted();
                yield return delay;
                cachedWidget.ShowDefault();

                yield return delay;
            }
        }
        
        private void OnRoundQuestionSelected(string questionId)
        {
            StartCoroutine(ShowQuestionBlinkEffect(questionId));
        }
        
        private void OnRoundInfoClicked(int number)
        {
            MatchSystem.SelectRound(number);
        }

        public void OnMatchSettingsButtonClicked()
        {
            MatchSettingsView.Show();
        }
    }
}