using Injection;
using MLAPI;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina.DevTools
{
    public class DisconnectPanelView : ViewBase
    {
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        [Inject] private NetworkingManager NetworkingManager { get; set; }
        
        public Text[] Names;
        
        public void Initialize()
        {
            MetagameEvents.MasterClientConnected.Subscribe(_ => RefreshUI());
            MetagameEvents.MasterClientDisconnected.Subscribe(RefreshUI);
        }

        protected override void OnShown()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            for (int i = 0; i < Names.Length; i++)
            {
                bool hasPlayer = ConnectedPlayersData.Players.Count > i;
                Names[i].transform.parent.gameObject.SetActive(hasPlayer);
                if (hasPlayer)
                    Names[i].text = ConnectedPlayersData.Players[i].Name;
            }
        }

        public void OnDisconnectPlayerButtonClicked(int index)
        {
            Debug.Log($"Debug: disconnect: {index}");
            ulong clientId = ConnectedPlayersData.Players[index].ClientId;
            NetworkingManager.DisconnectClient(clientId);
        }

        public void Update()
        {
            if (Static.DebugSettings.DisconnectPanel && !IsActive && Static.BuildMode == BuildMode.Development)
                Show();

            if (!Static.DebugSettings.DisconnectPanel && IsActive)
                Hide();
        }
    }
}