using System;
using Assets.Scripts.Utils;
using Injection;
using MLAPI;
using UnityEngine;

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
            _injector.Bind(FindObjectOfType<LobbyView>());
            _injector.Bind(FindObjectOfType<TextStoryDotView>());
            _injector.Bind(FindObjectOfType<ImageStoryDotView>());
            _injector.Bind(FindObjectOfType<AudioStoryDotView>());
            _injector.Bind(FindObjectOfType<VideoStoryDotView>());
            _injector.Bind(FindObjectOfType<NoRiskStoryDotView>());
            _injector.Bind(FindObjectOfType<RoundView>());
            _injector.Bind(FindObjectOfType<PlayersButtonClickPanelView>());
            _injector.Bind(FindObjectOfType<PlayersBoardView>());
            _injector.Bind(FindObjectOfType<MatchSettingsView>());
            
            _injector.Bind(new QuestionTimer());
            
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
            _injector.Bind(matchData.QuestionAnswerData);

            _injector.Bind(new CatInBagSystem());
            _injector.Bind(FindObjectOfType<CatInBagStoryDotView>());
            _injector.Bind(FindObjectOfType<CatInBagData>());

            _injector.Bind(new AuctionSystem());
            _injector.Bind(FindObjectOfType<AuctionStoryDotView>());
            
            _injector.Bind(new AnsweringTimerSystem());
            _injector.Bind(new AnsweringTimerData());
            _injector.Bind(FindObjectOfType<AnsweringTimerView>());
            
            _injector.Bind(new PackageSystem());
            _injector.Bind(new PackageData());
            
            _injector.Bind(new PathSystem());
            _injector.Bind(new PathData());
            
            _injector.Bind(NetworkingManager.Singleton);
            
            _injector.Bind(new DataChangeHandler());
            _injector.Bind(new PlayEffectsSystem());
            _injector.Bind(FindObjectOfType<PlayEffectsData>());
            
            //Master only
            _injector.Bind(new ServerService());
            _injector.Bind(new ConnectedPlayersData());
            _injector.Bind(new FilesDeliveryStatusManager());
            _injector.Bind(new PlayersBoardSystem());
            _injector.Bind(new QuestionAnswerSystem());
            _injector.Bind(new TimerRunOutDetectSystem());
            _injector.Bind(new MasterDataReceiver());
            _injector.Bind(new MasterContextKeyboardSystem());
            _injector.Bind(FindObjectOfType<MasterContextKeyboardTipView>());
            _injector.Bind(new MasterEffectsSystem());
            _injector.Bind(FindObjectOfType<MasterQuestionPanelView>());
            _injector.Bind(FindObjectOfType<MasterAcceptAnswerView>());
            _injector.Bind(FindObjectOfType<MasterPlayerSettingsView>());
            _injector.Bind(FindObjectOfType<MasterEffectsView>());
            _injector.Bind(FindObjectOfType<DataSyncService>());
            
            //Client only
            _injector.Bind(FindObjectOfType<DownloadingFilesPanelView>());
            _injector.Bind(FindObjectOfType<PlayerButtonView>());
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
            
            _injector.Bind(new SiqLoadedPackageSystem());
            _injector.Bind(new SiqLoadedPackageData());
            _injector.Bind(new SiqConverter());
            _injector.Bind(new EncodingFixSystem());
            
            _injector.Bind(new PackageFilesSystem());
            _injector.Bind(new PackageJsonConverter());
            
            //Crafter
            _injector.Bind(FindObjectOfType<PackageCrafterView>());
            _injector.Bind(new PackageCrafterSystem());
            _injector.Bind(new CrafterData());
            
            _injector.Bind(FindObjectOfType<CrafterQuestionPreview>());
            
            _injector.Bind(new CrafterDragAndDropSystem());
            _injector.Bind(FindObjectOfType<CrafterDragAndDropData>());
            
            _injector.Bind(FindObjectOfType<ThemesSelectionFromBagView>());
            _injector.Bind(new CrafterBagSystem());
            //End Crafter
            
            _injector.CommitBindings();
            
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
            _injector.Get<SaveSystem>().LoadAll();
            
            GeneratePlayerGuid();//after load app state
            
            StartCoroutine(_injector.Get<ExternalIpSystem>().Initialize());
            
            _injector.Get<PathSystem>().Initialize();
            
            _injector.Get<ServerService>().Initialize();
            _injector.Get<PlayersBoardSystem>().Initialize();
            
            _injector.Get<ClientService>().Initialize();
            
            _injector.Get<ViewsSystem>().Initialize();
            _injector.Get<DataSerializationService>().Initialize();
            
            _injector.Get<PlayersBoardView>().Initialize();
            _injector.Get<RoundView>().Initialize();
            _injector.Get<ImageStoryDotView>().Initialize();
            _injector.Get<AudioStoryDotView>().Initialize();
            _injector.Get<VideoStoryDotView>().Initialize();
            
            _injector.Get<PlayEffectsSystem>().Initialize();

            StartCoroutine(_injector.Get<FilesDeliveryStatusManager>().Initialize());
            _injector.Get<PlayerFilesRequestSystem>().Initialize();
            _injector.Get<PlayersButtonClickPanelView>().Initialize();
            
            _injector.Get<DownloadingFilesPanelView>().Initialize();
            _injector.Get<PlayerButtonView>().Initialize();

            _injector.Get<MatchSystem>().Initialize();
            
            _injector.Get<MasterQuestionPanelView>().Initialize();
            _injector.Get<MasterEffectsView>().Initialize();
            _injector.Get<DataSyncService>().Initialize();
            _injector.Get<MasterContextKeyboardTipView>().Initialize();
            
            _injector.Get<CatInBagSystem>().Initialize();
            _injector.Get<CatInBagStoryDotView>().Initialize();
            
            _injector.Get<AuctionStoryDotView>().Initialize();
            
            _injector.Get<AnsweringTimerSystem>().Initialize();
            
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
            _injector.Get<AnsweringTimerSystem>().OnUpdate();
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
    }
}