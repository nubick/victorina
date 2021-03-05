using Injection;
using UnityEngine;

namespace Victorina
{
    public class SaveSystem
    {
        [Inject] private AppState AppState { get; set; }

        private const string LastJoinPlayerNameKey = "LastJoinPlayerNameKey";
        private const string LastJoinGameCodeKey = "LastJoinGameCodeKey";
        private const string VolumeKey = "VolumeKey";
        private const string PlayerGuidKey = "PlayerGuidKey";
        
        public void LoadAll()
        {
            Load(AppState);
        }

        private void Load(AppState appState)
        {
            appState.LastJoinPlayerName = PlayerPrefs.GetString(LastJoinPlayerNameKey);
            appState.LastJoinGameCode = PlayerPrefs.GetString(LastJoinGameCodeKey);
            appState.Volume.Value = PlayerPrefs.GetFloat(VolumeKey, 1f);
            appState.PlayerGuid = PlayerPrefs.GetString(PlayerGuidKey);
        }
        
        private void Save(AppState appState)
        {
            PlayerPrefs.SetString(LastJoinPlayerNameKey, appState.LastJoinPlayerName);
            PlayerPrefs.SetString(LastJoinGameCodeKey, appState.LastJoinGameCode);
            PlayerPrefs.SetFloat(VolumeKey, appState.Volume.Value);
            PlayerPrefs.SetString(PlayerGuidKey, appState.PlayerGuid);
        }

        public void Save()
        {
            Save(AppState);
        }
    }
}