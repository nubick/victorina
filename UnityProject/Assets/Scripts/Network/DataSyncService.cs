using System.Collections;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class DataSyncService : MonoBehaviour
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private FinalRoundData FinalRoundData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PackagePlayStateData PackagePlayStateData { get; set; }
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        [Inject] private AnswerTimerData AnswerTimerData { get; set; }

        public void Initialize()
        {
            MetagameEvents.ServerStarted.Subscribe(OnServerStarted);
            MetagameEvents.ServerStopped.Subscribe(OnServerStopped);
        }

        private void OnServerStarted()
        {
            StopAllCoroutines();
            StartCoroutine(SynchronizeCoroutine());
        }

        private void OnServerStopped()
        {
            StopAllCoroutines();
        }

        private IEnumerator SynchronizeCoroutine()
        {
            WaitForSeconds delay = new WaitForSeconds(0.1f);
            for (;;)
            {
                if (PlayersButtonClickData.HasChanges)
                {
                    Debug.Log($"DataSync: {PlayersButtonClickData}");
                    PlayersButtonClickData.ApplyChanges();
                    SendToPlayersService.SendPlayersButtonClickData(PlayersButtonClickData);
                }

                if (FinalRoundData.HasChanges)
                {
                    Debug.Log($"DataSync: {FinalRoundData}");
                    FinalRoundData.ApplyChanges();
                    SendToPlayersService.SendFinalRoundData(FinalRoundData);
                    MetagameEvents.FinalRoundDataChanged.Publish();
                }

                if (PlayersBoard.HasChanges)
                {
                    //Debug.Log("DataSync: PlayersBoard");
                    PlayersBoard.ApplyChanges();
                    SendToPlayersService.Send(PlayersBoard);
                    MetagameEvents.PlayersBoardChanged.Publish();
                }
                
                if (PackagePlayStateData.HasChanges)
                {
                    Debug.Log($"DataSync: {PackagePlayStateData}");
                    PackagePlayStateData.ApplyChanges();
                    SendToPlayersService.SendPackagePlayStateData(PackagePlayStateData);
                    MetagameEvents.PackagePlayStateChanged.Publish();
                }

                if (AnswerTimerData.HasChanges)
                {
                    Debug.Log($"DataSync: {AnswerTimerData}");
                    AnswerTimerData.ApplyChanges();
                    SendToPlayersService.SendAnswerTimerData(AnswerTimerData);
                    MetagameEvents.AnswerTimerDataChanged.Publish();
                }
                
                yield return delay;
            }
        }
    }
}