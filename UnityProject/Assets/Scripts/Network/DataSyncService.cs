using System.Collections;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class DataSyncService : MonoBehaviour
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private PlayersButtonClickData PlayersButtonClickData { get; set; }
        [Inject] private AnswerTimerData AnswerTimerData { get; set; }
        [Inject] private ServerEventsData ServerEventsData { get; set; }

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
                    MetagameEvents.PlayersButtonClickDataChanged.Publish();
                }
                
                if (PlayersBoard.HasChanges)
                {
                    //Debug.Log("DataSync: PlayersBoard");
                    PlayersBoard.ApplyChanges();
                    SendToPlayersService.Send(PlayersBoard);
                    MetagameEvents.PlayersBoardChanged.Publish();
                }
                
                if (PlayStateData.HasChanges)
                {
                    Debug.Log($"DataSync: {PlayStateData}");
                    PlayStateData.ApplyChanges();
                    SendToPlayersService.SendPackagePlayStateData(PlayStateData);
                    MetagameEvents.PlayStateChanged.Publish();
                }

                if (AnswerTimerData.HasChanges)
                {
                    Debug.Log($"DataSync: {AnswerTimerData}");
                    AnswerTimerData.ApplyChanges();
                    SendToPlayersService.SendAnswerTimerData(AnswerTimerData);
                    MetagameEvents.AnswerTimerDataChanged.Publish();
                }

                while (ServerEventsData.PendingToSendEvents.Any())
                {
                    Debug.Log($"DataSync: {ServerEventsData}");
                    (string, ServerEventArgument) serverEvent = ServerEventsData.PendingToSendEvents.Dequeue();
                    SendToPlayersService.SendServerEvent(serverEvent.Item1, serverEvent.Item2);
                }
                
                yield return delay;
            }
        }
    }
}