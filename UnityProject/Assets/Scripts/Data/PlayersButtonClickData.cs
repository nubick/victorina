using System.Collections.Generic;
using System.Linq;

namespace Victorina
{
    public class PlayersButtonClickData
    {
        public List<PlayerButtonClickData> Players { get; } = new List<PlayerButtonClickData>();

        public bool HasChanges { get; private set; }

        public void ApplyChanges()
        {
            HasChanges = false;
        }
        
        public void Add(byte playerId, string name, float spentSeconds)
        {
            PlayerButtonClickData clickData = new PlayerButtonClickData();
            clickData.PlayerId = playerId;
            clickData.Name = name;
            clickData.Time = spentSeconds;
            Players.Add(clickData);
            HasChanges = true;
            MetagameEvents.PlayersButtonClickDataChanged.Publish();
        }
        
        public void Clear()
        {
            if (Players.Any())
            {
                Players.Clear();
                HasChanges = true;
                MetagameEvents.PlayersButtonClickDataChanged.Publish();
            }
        }
        
        public override string ToString()
        {
            return $"[{string.Join("|", Players.Select(_ => $"{_.Name}:{_.Time:0.00}"))}]";
        }
    }
}