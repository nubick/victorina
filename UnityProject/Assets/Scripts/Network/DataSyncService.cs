using System.Collections;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class DataSyncService : MonoBehaviour
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        
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
                    QuestionAnswerData.PlayersButtonClickData.ApplyChanges();
                    SendToPlayersService.SendPlayersButtonClickData(QuestionAnswerData.PlayersButtonClickData);
                }
                yield return delay;
            }
        }
    }
}