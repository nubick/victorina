using System.Text;
using Injection;
using MLAPI;
using UnityEngine;

namespace Victorina
{
    public class ClientService
    {
        [Inject] private NetworkingManager NetworkingManager { get; set; }
        [Inject] private RightsData RightsData { get; set; }
        
        public void JoinGame(string playerName)
        {
            Debug.Log($"JoinGame: '{playerName}'");
            byte[] login = Encoding.UTF32.GetBytes(playerName);
            NetworkingManager.NetworkConfig.ConnectionData = login;
            NetworkingManager.StartClient();
            RightsData.IsAdmin = false;
        }
    }
}