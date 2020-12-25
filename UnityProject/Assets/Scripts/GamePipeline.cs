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
            _injector.Bind(FindObjectOfType<TextQuestionView>());
            
            _injector.Bind(new RightsData());
            
            _injector.Bind(new AppState());
            _injector.Bind(new SaveSystem());
            
            _injector.Bind(new ExternalIpSystem());
            _injector.Bind(new ExternalIpData());
            
            _injector.Bind(new IpCodeSystem());
            
            _injector.Bind(new GameLobbySystem());
            _injector.Bind(new GameLobbyData());
            
            _injector.Bind(new MatchService());
            _injector.Bind(new MatchData());
            
            
            _injector.Bind(NetworkingManager.Singleton);
            _injector.Bind(new ServerService());
            _injector.Bind(new ClientService());
            
            _injector.Bind(new SendToPlayersService());
            _injector.Bind(new DataSerializationService());
            
            _injector.CommitBindings();
        }

        private void Initialize()
        {
            _injector.Get<SaveSystem>().LoadAll();
            
            StartCoroutine(_injector.Get<ExternalIpSystem>().Initialize());
            _injector.Get<ServerService>().Initialize();
            
            _injector.Get<GameLobbySystem>().Initialize();
            _injector.Get<ViewsSystem>().Initialize();
            _injector.Get<DataSerializationService>().Initialize();
        }
        
        private void OnPlayerConnected(NetworkPlayer networkPlayer)
        {
            _injector.InjectTo(networkPlayer);
        }
    }
}