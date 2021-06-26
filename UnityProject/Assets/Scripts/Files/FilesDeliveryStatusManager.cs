using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class FilesDeliveryStatusManager
    {
        [Inject] private PackageSystem PackageSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        
        private readonly Dictionary<byte, HashSet<int>> _playerIdToFilesMap = new Dictionary<byte, HashSet<int>>();
        private bool _isSyncRequired;
        
        public IEnumerator Initialize()
        {
            MetagameEvents.ServerStarted.Subscribe(Clear);
            MetagameEvents.ServerStopped.Subscribe(Clear);
            MetagameEvents.MasterClientConnected.Subscribe(RegisterPlayer);
            yield return SyncCurrentRoundCoroutine();
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
            _isSyncRequired = true;
        }

        private IEnumerator SyncCurrentRoundCoroutine()
        {
            for (;;)
            {
                yield return new WaitForSeconds(0.5f);

                if (_isSyncRequired && PlayStateData.Type == PlayStateType.Round)
                {
                    NetRound netRound = PlayStateData.As<RoundPlayState>().NetRound;
                    UpdateIsDownloadedByAll(netRound);
                    PlayStateData.MarkAsChanged();
                    _isSyncRequired = false;
                }
            }
        }

        private void UpdateIsDownloadedByAll(NetRound netRound)
        {
            var questions = netRound.Themes.SelectMany(theme => theme.Questions);
            foreach (NetRoundQuestion netRoundQuestion in questions)
                netRoundQuestion.IsDownloadedByAll = IsDownloadedByAll(netRoundQuestion.FileIds);
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