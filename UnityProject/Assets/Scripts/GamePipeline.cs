using System;
using Assets.Scripts.Utils;
using Injection;
using MLAPI;
using UnityEngine;
using Victorina.Commands;
using Victorina.DevTools;
using Debug = UnityEngine.Debug;

namespace Victorina
{
    public class GamePipeline : MonoBehaviour
    {
        private Injector _injector;
        
        public void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            
            _injector = new Injector();
            InjectAll();//InjectAll from Start as from Awake NetworkingManager.Singleton is still null
            RegisterHandlers();
            Initialize();
            MetagameEvents.NetworkPlayerSpawned.Subscribe(OnNetworkPlayerSpawned);
        }
        
        private void InjectAll()
        {
            _injector.Bind(new ViewsSystem());
            
            //Views
            ViewsData viewsData = FindObjectOfType<ViewsData>();
            _injector.Bind(viewsData);
            _injector.Bind(FindObjectOfType<StartupView>());
            _injector.Bind(FindObjectOfType<JoinGameView>());
            _injector.Bind(FindObjectOfType<CreatePackageGameView>());
            _injector.Bind(new CreatePackageGameSystem());
            _injector.Bind(new CreatePackageGameData());
            _injector.Bind(FindObjectOfType<LobbyView>());
            _injector.Bind(FindObjectOfType<TextStoryDotView>());
            _injector.Bind(FindObjectOfType<ImageStoryDotView>());
            _injector.Bind(FindObjectOfType<AudioStoryDotView>());
            _injector.Bind(FindObjectOfType<VideoStoryDotView>());
            _injector.Bind(FindObjectOfType<NoRiskView>());
            _injector.Bind(FindObjectOfType<RoundView>());
            _injector.Bind(FindObjectOfType<PlayersButtonClickPanelView>());
            _injector.Bind(FindObjectOfType<PlayersBoardView>());
            _injector.Bind(FindObjectOfType<MatchSettingsView>());
            
            _injector.Bind(new PlayStateSystem());
            _injector.Bind(new PlayStateData());
            
            _injector.Bind(new NetworkData());
            
            _injector.Bind(new AppState());
            _injector.Bind(new SaveSystem());
            
            _injector.Bind(new MatchSettingsData());
            
            _injector.Bind(new ExternalIpSystem());
            _injector.Bind(new ExternalIpData());
            
            _injector.Bind(new IpCodeSystem());
            
            _injector.Bind(new MatchSystem());
            MatchData matchData = new MatchData();
            _injector.Bind(matchData);
            _injector.Bind(new PlayersBoard());

            _injector.Bind(new CatInBagSystem());
            _injector.Bind(FindObjectOfType<CatInBagView>());

            _injector.Bind(new AuctionSystem());
            _injector.Bind(FindObjectOfType<AuctionView>());
            
            _injector.Bind(FindObjectOfType<PlayersMoreInfoView>());
            _injector.Bind(new PlayersMoreInfoData());
            
            _injector.Bind(new AcceptingAnswerTimerSystem());
            _injector.Bind(new AcceptingAnswerTimerData());
            _injector.Bind(FindObjectOfType<AcceptingAnswerTimerView>());
            
            _injector.Bind(new PackageSystem());
            _injector.Bind(new PackageData());
            
            _injector.Bind(new PathSystem());
            _injector.Bind(new PathData());
            
            _injector.Bind(NetworkingManager.Singleton);
            
            _injector.Bind(new PlayEffectsSystem());
            _injector.Bind(FindObjectOfType<PlayEffectsData>());
            
            _injector.Bind(new CommandsSystem());
            
            //Show Question
            _injector.Bind(FindObjectOfType<MasterShowQuestionView>());
            _injector.Bind(FindObjectOfType<PlayerGiveAnswerView>());
            _injector.Bind(new AcceptAnswerSystem());
            _injector.Bind(new ShowQuestionSystem());
            _injector.Bind(new PlayersButtonClickData());
            _injector.Bind(new TimerSystem());  //--- Timer
            _injector.Bind(new QuestionStripTimer());
            _injector.Bind(FindObjectOfType<TimerCoroutinesContainer>());
            _injector.Bind(new AnswerTimerSystem());
            _injector.Bind(new AnswerTimerData());
            _injector.Bind(new TimerRunOutDetectSystem());
            _injector.Bind(new MasterAnswerTipData());  //--- Tip
            _injector.Bind(new MasterAnswerTipSystem());
            
            //Accepting Answer
            _injector.Bind(FindObjectOfType<MasterAcceptAnswerView>());
            _injector.Bind(FindObjectOfType<PlayerAcceptingAnswerView>());

            //Show Answer
            _injector.Bind(FindObjectOfType<MasterShowAnswerView>());
            _injector.Bind(FindObjectOfType<PlayerLookAnswerView>());
            _injector.Bind(new ShowAnswerSystem());
            
            //Final Round
            _injector.Bind(FindObjectOfType<FinalRoundView>());
            _injector.Bind(FindObjectOfType<MasterShowFinalRoundQuestionView>());
            _injector.Bind(new FinalRoundSystem());

            //Master Only
            _injector.Bind(new ServerService());
            _injector.Bind(new ConnectedPlayersData());
            _injector.Bind(new FilesDeliveryStatusManager());
            _injector.Bind(new PlayersBoardSystem());
            _injector.Bind(new MasterDataReceiver());
            _injector.Bind(new MasterContextKeyboardSystem());
            _injector.Bind(FindObjectOfType<MasterContextKeyboardTipView>());
            _injector.Bind(new MasterEffectsSystem());
            _injector.Bind(FindObjectOfType<MasterPlayerSettingsView>());
            _injector.Bind(FindObjectOfType<MasterEffectsView>());
            _injector.Bind(FindObjectOfType<DataSyncService>());
            
            //Master Only - Debug
            _injector.Bind(FindObjectOfType<DisconnectPanelView>());
            
            //Client only
            _injector.Bind(new PlayerDataReceiver());
            _injector.Bind(new PlayerAnswerSystem());

            _injector.Bind(new ClientService());
            
            _injector.Bind(new SendToPlayersService());
            _injector.Bind(new SendToMasterService());
            _injector.Bind(new DataSerializationService());
            
            _injector.Bind(new MasterFilesRepository());
            _injector.Bind(new PlayerFilesRepository());
            _injector.Bind(new PlayerFilesRequestSystem());
            _injector.Bind(FindObjectOfType<PlayerFilesRequestData>());
            
            _injector.Bind(new SiqConverter());
            
            _injector.Bind(new PackageFilesSystem());
            _injector.Bind(new PackageJsonConverter());
            
            _injector.Bind(FindObjectOfType<InputDialogueView>());
            _injector.Bind(FindObjectOfType<MessageDialogueView>());
            _injector.Bind(FindObjectOfType<ConfirmationDialogueView>());

            //DevTools
            _injector.Bind(new DevToolsSystem());
            _injector.Bind(new AnalyticsSystem());
            _injector.Bind(new LogsTrackingSystem());
            _injector.Bind(new LogsTrackingData());
            
            //Crafter
            _injector.Bind(FindObjectOfType<PackageCrafterView>());
            _injector.Bind(new PackageCrafterSystem());
            _injector.Bind(new CrafterData());
            
            _injector.Bind(FindObjectOfType<CrafterQuestionPreview>());
            _injector.Bind(FindObjectOfType<CrafterQuestionEditView>());
            _injector.Bind(FindObjectOfType<RoundAutoPriceInputView>());
            _injector.Bind(FindObjectOfType<SettingsTabsView>());
            
            _injector.Bind(new CrafterDragAndDropSystem());
            _injector.Bind(FindObjectOfType<CrafterDragAndDropData>());
            
            _injector.Bind(FindObjectOfType<ThemesSelectionFromBagView>());
            _injector.Bind(new CrafterBagSystem());
            //End Crafter

            Stopwatch.Start("CommitBindings");
            _injector.CommitBindings();
            Stopwatch.Stop();

            VolumeSettingsWidget[] widgets = viewsData.ViewsRoot.GetComponentsInChildren<VolumeSettingsWidget>(includeInactive: true);
            widgets.ForEach(_ => _injector.InjectTo(_));
        }

        private void RegisterHandlers()
        {
            KeyboardManager keyboardManager = FindObjectOfType<KeyboardManager>();
            foreach(object item in _injector.GetAll())
                if (item is IKeyPressedHandler handler)
                    keyboardManager.Register(handler);
        }
        
        private void Initialize()
        {
            _injector.Get<LogsTrackingSystem>().Initialize();

            _injector.Get<SaveSystem>().LoadAll();
            
            GeneratePlayerGuid();//after load app state
            
            StartCoroutine(_injector.Get<ExternalIpSystem>().Initialize());
            
            _injector.Get<PathSystem>().Initialize();
            
            _injector.Get<ServerService>().Initialize();
            _injector.Get<PlayersBoardSystem>().Initialize();
            
            _injector.Get<ClientService>().Initialize();
            
            _injector.Get<ViewsSystem>().Initialize();
            _injector.Get<DataSerializationService>().Initialize();
            
            _injector.Get<CreatePackageGameView>().Initialize();
            _injector.Get<PlayersBoardView>().Initialize();
            _injector.Get<RoundView>().Initialize();
            _injector.Get<ImageStoryDotView>().Initialize();
            _injector.Get<AudioStoryDotView>().Initialize();
            _injector.Get<VideoStoryDotView>().Initialize();
            
            _injector.Get<AnswerTimerSystem>().Initialize();
            
            _injector.Get<PlayEffectsSystem>().Initialize();

            StartCoroutine(_injector.Get<FilesDeliveryStatusManager>().Initialize());
            _injector.Get<PlayerFilesRequestSystem>().Initialize();
            _injector.Get<PlayersButtonClickPanelView>().Initialize();
            
            _injector.Get<MatchSystem>().Initialize();
            
            _injector.Get<MasterShowQuestionView>().Initialize();
            _injector.Get<PlayerGiveAnswerView>().Initialize();
            _injector.Get<MasterAnswerTipSystem>().Initialize();

            _injector.Get<MasterEffectsView>().Initialize();
            _injector.Get<DataSyncService>().Initialize();
            _injector.Get<MasterContextKeyboardTipView>().Initialize();
            
            _injector.Get<CatInBagView>().Initialize();
            
            _injector.Get<AuctionView>().Initialize();
            _injector.Get<PlayersMoreInfoView>().Initialize();
            _injector.Get<AcceptingAnswerTimerSystem>().Initialize();
            _injector.Get<FinalRoundView>().Initialize();

            _injector.Get<CommandsSystem>().Initialize(_injector);
            _injector.Get<PlayStateSystem>().Initialize(_injector);

            //DevTools
            //Analytics
            _injector.Get<AnalyticsSystem>().Initialize();

            //Debug
            _injector.Get<DisconnectPanelView>().Initialize();
            
            
            //Crafter
            _injector.Get<PackageCrafterView>().Initialize();
            _injector.Get<ThemesSelectionFromBagView>().Initialize();
            _injector.Get<CrafterQuestionPreview>().Initialize();
            _injector.Get<CrafterDragAndDropSystem>().Initialize();
        }
        
        private void OnNetworkPlayerSpawned(NetworkPlayer networkPlayer)
        {
            Debug.Log($"Pipeline: NetworkPlayer spawned, inject to networkPlayer:{networkPlayer.OwnerClientId}");
            _injector.InjectTo(networkPlayer);
        }

        public void Update()
        {
            _injector.Get<TimerRunOutDetectSystem>().OnUpdate();
            _injector.Get<AcceptingAnswerTimerSystem>().OnUpdate();
        }

        private void GeneratePlayerGuid()
        {
            AppState appState = _injector.Get<AppState>();
            if (Static.BuildMode == BuildMode.Development)
            {
                appState.PlayerGuid = Guid.NewGuid().ToString();
            }
            else
            {
                if (string.IsNullOrEmpty(appState.PlayerGuid))
                {
                    appState.PlayerGuid = Guid.NewGuid().ToString();
                    Debug.Log($"Generate player guid: {appState.PlayerGuid}");
                    _injector.Get<SaveSystem>().Save();
                }
            }
        }

        private void OnDestroy()
        {
            Debug.Log($"GamePipeline.OnDestroy");
            _injector.Get<AnalyticsSystem>().OnDestroy();
        }
    }
}