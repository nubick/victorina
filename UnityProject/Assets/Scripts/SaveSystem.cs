using Injection;
using UnityEngine;

namespace Victorina
{
    public class SaveSystem
    {
        [Inject] private AppState AppState { get; set; }

        private const string LastJoinPlayerNameKey = "LastJoinPlayerNameKey";
        private const string LastJoinGameCodeKey = "LastJoinGameCodeKey";
        
        public void LoadAll()
        {
            Load(AppState);
        }

        private void Load(AppState appState)
        {
            appState.LastJoinPlayerName = PlayerPrefs.GetString(LastJoinPlayerNameKey);
            appState.LastJoinGameCode = PlayerPrefs.GetString(LastJoinGameCodeKey);
        }
        
        private void Save(AppState appState)
        {
            PlayerPrefs.SetString(LastJoinPlayerNameKey, appState.LastJoinPlayerName);
            PlayerPrefs.SetString(LastJoinGameCodeKey, appState.LastJoinGameCode);
        }

        public void Save()
        {
            Save(AppState);
        }
    }
}