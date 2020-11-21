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
        }

        private void InjectAll()
        {
            _injector.Bind(FindObjectOfType<StartupView>());
            _injector.Bind(FindObjectOfType<HostView>());
            _injector.Bind(FindObjectOfType<JoinGameView>());
            _injector.Bind(FindObjectOfType<GameLobbyView>());
            
            _injector.Bind(new GameLobbySystem());
            _injector.Bind(new GameLobbyData());
            
            _injector.Bind(NetworkingManager.Singleton);
            _injector.Bind(new ServerService());
            _injector.Bind(new ClientService());
            
            _injector.CommitBindings();
        }

        private void Initialize()
        {
            InitializeViews();
            
            _injector.Get<ServerService>().Initialize();
            _injector.Get<GameLobbySystem>().Initialize();
        }
        
        private void InitializeViews()
        {
            foreach(ViewBase view in FindObjectsOfType<ViewBase>())
                view.Content.SetActive(false);
            _injector.Get<StartupView>().Show();
        }
    }
}