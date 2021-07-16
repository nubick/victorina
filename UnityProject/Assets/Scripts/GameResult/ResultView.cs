using System.Collections.Generic;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class ResultView : ViewBase
    {
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        
        public GameObject LobbyButton;

        public RectTransform PlayerLinesRoot;
        public ResultPlayerLineWidget PlayerLinePrefab;
        public Color WinnerNameColor;
        public Color PlayerNameColor;
        
        protected override void OnShown()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            (List<PlayerData> Winners, List<PlayerData> Players) splitPlayers = SplitPlayers(PlayersBoard.Players);
            RefreshPlayerLines(splitPlayers.Winners, splitPlayers.Players);
            
            LobbyButton.SetActive(NetworkData.IsMaster);
        }

        private void RefreshPlayerLines(List<PlayerData> winners, List<PlayerData> players)
        {
            ClearChild(PlayerLinesRoot);

            for (int i = 0; i < winners.Count; i++)
            {
                ResultPlayerLineWidget widget = Instantiate(PlayerLinePrefab, PlayerLinesRoot);
                string winnerLabel = winners.Count > 1 ? "Победители" : "Победитель";
                string resultLabel = i == 0 ? winnerLabel : string.Empty;
                widget.Bind(resultLabel, winners[i].Name, winners[i].Score, WinnerNameColor);
            }

            for (int i = 0; i < players.Count; i++)
            {
                ResultPlayerLineWidget widget = Instantiate(PlayerLinePrefab, PlayerLinesRoot);
                string resultLabel = i == 0 ? "Игроки" : string.Empty;
                widget.Bind(resultLabel, players[i].Name, players[i].Score, PlayerNameColor);
            }
        }

        private (List<PlayerData> Winners, List<PlayerData> Players) SplitPlayers(List<PlayerData> allPlayers)
        {
            List<PlayerData> winners = new List<PlayerData>();
            int maxScore = allPlayers.Max(_ => _.Score);
            if (maxScore > 0)
                winners = allPlayers.Where(_ => _.Score == maxScore).ToList();
            List<PlayerData> players = allPlayers.Except(winners).OrderByDescending(_ => _.Score).ToList();
            return (winners, players);
        }

        public void OnLobbyButtonClicked()
        {
            PlayStateSystem.ChangePlayState(new LobbyPlayState());
        }
    }
}