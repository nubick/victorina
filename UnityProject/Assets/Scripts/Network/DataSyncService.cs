using System.Collections;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class DataSyncService : MonoBehaviour
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private FinalRoundData FinalRoundData { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }

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
                if (QuestionAnswerData.PlayersButtonClickData.HasChanges)
                {
                    Debug.Log("DataSync: PlayersButtonClickData");
                    QuestionAnswerData.PlayersButtonClickData.ApplyChanges();
                    SendToPlayersService.SendPlayersButtonClickData(QuestionAnswerData.PlayersButtonClickData);
                }

                if (FinalRoundData.HasChanges)
                {
                    Debug.Log("DataSync: FinalRoundData");
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
                
                yield return delay;
            }
        }
    }
}