using System.Collections.Generic;
using System.Linq;

namespace Victorina
{
    public class PlayersButtonClickData : SyncData
    {
        public List<PlayerButtonClickData> Players { get; } = new List<PlayerButtonClickData>();
        
        public void Add(byte playerId, string name, float spentSeconds)
        {
            PlayerButtonClickData clickData = new PlayerButtonClickData();
            clickData.PlayerId = playerId;
            clickData.Name = name;
            clickData.Time = spentSeconds;
            Players.Add(clickData);
            MarkAsChanged();
            MetagameEvents.PlayersButtonClickDataChanged.Publish();
        }
        
        public void Clear()
        {
            if (Players.Any())
            {
                Players.Clear();
                MarkAsChanged();
                MetagameEvents.PlayersButtonClickDataChanged.Publish();
            }
        }

        public void Update(PlayersButtonClickData data)
        {
            Players.Clear();
            Players.AddRange(data.Players);
        }
        
        public override string ToString()
        {
            return $"[{string.Join("|", Players.Select(_ => $"{_.Name}:{_.Time:0.00}"))}]";
        }
    }
}