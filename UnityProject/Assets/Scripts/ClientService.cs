using System;
using System.Net;
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
        [Inject] private ExternalIpData ExternalIpData { get; set; }
        
        public void JoinGame(string playerName, string ip)
        {
            Debug.Log($"JoinGame: '{playerName}', ip: {ip}");

            UnetTransport transport = NetworkingManager.GetComponent<UnetTransport>();

            IPAddress ipAddress;
            if (IPAddress.TryParse(ip, out ipAddress))
            {
                transport.ConnectAddress = ipAddress.ToString();
                transport.ConnectPort = 139;
                
                byte[] login = Encoding.UTF32.GetBytes(playerName);
                NetworkingManager.NetworkConfig.ConnectionData = login;
                
                
                NetworkingManager.StartClient();
                RightsData.IsAdmin = false;
            }
            else
            {
                throw new Exception($"Can't parse ip: {ip}");
            }
        }
    }
}