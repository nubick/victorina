using System.Collections.Generic;
using System.Linq;
using Injection;

namespace Victorina
{
    public class FilesDeliveryStatusManager
    {
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        
        private readonly Dictionary<byte, HashSet<int>> _playerIdToFilesMap = new Dictionary<byte, HashSet<int>>();
        
        public void Initialize()
        {
            MetagameEvents.ServerStarted.Subscribe(Clear);
            MetagameEvents.ServerStopped.Subscribe(Clear);
            MetagameEvents.MasterClientConnected.Subscribe(RegisterPlayer);
        }

        private void Clear()
        {
            _playerIdToFilesMap.Clear();
        }
        
        private void RegisterPlayer(byte playerId)
        {
            if(!_playerIdToFilesMap.ContainsKey(playerId))
                _playerIdToFilesMap.Add(playerId, new HashSet<int>());
        }

        public void UpdateDownloadedFilesIds(byte playerId, int[] downloadedFilesIds)
        {
            HashSet<int> hashSet = _playerIdToFilesMap[playerId];
            hashSet.Clear();
            hashSet.UnionWith(downloadedFilesIds);

            if (MatchData.Phase.Value == MatchPhase.Round)
            {
                MatchSystem.SyncCurrentRound();
                MatchData.RoundData.NotifyChanged();
            }
        }

        public int[] GetQuestionFileIds(Question question)
        {
            return PackageSystem.GetFileStoryDots(question).Select(_ => _.FileId).ToArray();
        }
        
        public bool IsDownloadedByAll(int[] fileIds)
        {
            return fileIds.All(IsDownloadedByAll);
        }

        private bool IsDownloadedByAll(int fileId)
        {
            return _playerIdToFilesMap.Values.All(hashSet => hashSet.Contains(fileId));
        }
    }
}