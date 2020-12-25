using System.Text;
using Injection;
using MLAPI;
using MLAPI.Transports.UNET;
using UnityEngine;

namespace Victorina
{
    public class ClientService
    {
        [Inject] private NetworkingManager NetworkingManager { get; set; }
        [Inject] private RightsData RightsData { get; set; }
        [Inject] private IpCodeSystem IpCodeSystem { get; set; }
        
        public void JoinGame(string playerName, string gameCode)
        {
            Debug.Log($"JoinGame: '{playerName}', game code: {gameCode}");
            
            string ip = IpCodeSystem.GetIp(gameCode);
            Debug.Log($"Joining IP: {ip}");

            UnetTransport transport = NetworkingManager.GetComponent<UnetTransport>();
            transport.ConnectAddress = ip;
            transport.ConnectPort = Static.Port;
            
            byte[] playerNameBytes = Encoding.UTF32.GetBytes(playerName);
            NetworkingManager.NetworkConfig.ConnectionData = playerNameBytes;
            
            NetworkingManager.StartClient();
            
            RightsData.IsAdmin = false;
        }
    }
}