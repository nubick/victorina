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
            _injector = new Injector();
            InjectAll();//InjectAll from Start as from Awake NetworkingManager.Singleton is still null
            Initialize();
            MetagameEvents.PlayerConnected.Subscribe(OnPlayerConnected);
        }
        
        private void InjectAll()
        {
            _injector.Bind(new ViewsSystem());
            
            _injector.Bind(FindObjectOfType<StartupView>());
            _injector.Bind(FindObjectOfType<JoinGameView>());
            _injector.Bind(FindObjectOfType<GameLobbyView>());
            _injector.Bind(FindObjectOfType<TextStoryDotView>());
            _injector.Bind(FindObjectOfType<ImageStoryDotView>());
            _injector.Bind(FindObjectOfType<AudioStoryDotView>());
            _injector.Bind(FindObjectOfType<VideoStoryDotView>());
            _injector.Bind(FindObjectOfType<RoundView>());
            _injector.Bind(FindObjectOfType<MasterQuestionPanelView>());
            _injector.Bind(FindObjectOfType<DownloadingFilesPanelView>());
            
            _injector.Bind(FindObjectOfType<PlayerButtonView>());
            _injector.Bind(new QuestionTimer());
            
            _injector.Bind(new NetworkData());
            
            _injector.Bind(new AppState());
            _injector.Bind(new SaveSystem());
            
            _injector.Bind(new ExternalIpSystem());
            _injector.Bind(new ExternalIpData());
            
            _injector.Bind(new IpCodeSystem());
            
            _injector.Bind(new GameLobbySystem());
            
            _injector.Bind(new MatchSystem());
            _injector.Bind(new MatchData());
            
            _injector.Bind(new PackageSystem());
            _injector.Bind(new PackageData());
            
            _injector.Bind(NetworkingManager.Singleton);
            _injector.Bind(new ServerService());
            _injector.Bind(new ClientService());
            
            _injector.Bind(new SendToPlayersService());
            _injector.Bind(new SendToMasterService());
            _injector.Bind(new DataSerializationService());
            
            _injector.Bind(new MasterFilesRepository());
            _injector.Bind(new ClientFilesRepository());
            _injector.Bind(new ClientFilesRequestSystem());
            
            _injector.Bind(new SiqPackOpenSystem());
            _injector.Bind(new SiqLoadedPackageSystem());
            _injector.Bind(new SiqLoadedPackageData());
            _injector.Bind(new SiqConverter());
            _injector.Bind(new EncodingFixSystem());
            
            _injector.CommitBindings();
        }

        private void Initialize()
        {
            _injector.Get<SaveSystem>().LoadAll();
            
            StartCoroutine(_injector.Get<ExternalIpSystem>().Initialize());
            _injector.Get<ServerService>().Initialize();
            _injector.Get<ClientService>().Initialize();
            
            _injector.Get<GameLobbySystem>().Initialize();
            _injector.Get<ViewsSystem>().Initialize();
            _injector.Get<DataSerializationService>().Initialize();
            
            _injector.Get<RoundView>().Initialize();
            _injector.Get<ImageStoryDotView>().Initialize();
            _injector.Get<AudioStoryDotView>().Initialize();
            _injector.Get<VideoStoryDotView>().Initialize();

            StartCoroutine(_injector.Get<ClientFilesRequestSystem>().RequestCoroutine());
            _injector.Get<DownloadingFilesPanelView>().Initialize();
        }
        
        private void OnPlayerConnected(NetworkPlayer networkPlayer)
        {
            Debug.Log($"Pipeline: OnPlayerConnected, inject to networkPlayer:{networkPlayer.OwnerClientId}");
            _injector.InjectTo(networkPlayer);
        }
    }
}