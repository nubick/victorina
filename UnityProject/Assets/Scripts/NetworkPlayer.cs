using System;
using MLAPI;
using UnityEngine;

namespace Victorina
{
    public class NetworkPlayer : MonoBehaviour
    {
        public NetworkedObject NetworkedObject;
        
        public string PlayerName { get; set; }

        public void Awake()
        {
            Debug.Log("Network player created");
            
        }
    }
}