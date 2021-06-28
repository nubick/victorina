using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class RemoveFinalRoundThemeCommand : Command, INetworkCommand, IServerCommand
    {
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private PlayersBoard PlayersBoard { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        
        public int ThemeIndex { get; set; }
        
        public override CommandType Type => CommandType.RemoveFinalRoundTheme;
        private bool IsOwnerCurrentPlayerOrMaster => Owner == CommandOwner.Master || PlayersBoardSystem.IsCurrentPlayer(OwnerPlayer);
        private FinalRoundPlayState PlayState => PlayStateData.As<FinalRoundPlayState>();
        
        public bool CanSend()
        {
            if (!IsOwnerCurrentPlayerOrMaster)
            {
                Debug.Log($"Can't remove theme as owner '{OwnerString}' is not current '{PlayersBoard.Current}'");
                return false;
            }
            return true;
        }

        public bool CanExecuteOnServer()
        {
            if (!IsOwnerCurrentPlayerOrMaster)
            {
                Debug.Log($"Owner '{OwnerString}' can't remove theme '{ThemeIndex}' as they is not current");
                return false;
            }
            
            if (PlayState.RemainedThemesAmount <= 1)
            {
                Debug.Log($"Can't remove any theme anymore, remained themes amount: {PlayState.RemainedThemesAmount}");
                return false;
            }

            if (ThemeIndex < 0 || ThemeIndex >= PlayState.RemovedThemes.Length)
            {
                Debug.Log($"Not correct remove theme index '{ThemeIndex}'. Length is '{PlayState.RemovedThemes.Length}'");
                return false;
            }

            if (PlayState.RemovedThemes[ThemeIndex])
            {
                Debug.Log($"Theme by index '{ThemeIndex}' was removed before");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            Debug.Log($"Master. Remove theme '{ThemeIndex}' for player '{PlayersBoard.Current}' by '{OwnerString}'");
            PlayState.RemoveTheme(ThemeIndex);

            if (PlayState.IsAllThemesRemoved)
                PlayersBoard.SetCurrent(null);

            if (PlayersBoard.Current != null)
                SelectNextPlayer();
        }

        private void SelectNextPlayer()
        {
            int index = PlayersBoard.GetPlayerIndex(PlayersBoard.Current);
            int i = index;
            for (;;)
            {
                i = (i + 1) % PlayersBoard.Players.Count;
                
                if (i == index)
                    break;

                if (FinalRoundSystem.CanParticipate(PlayersBoard.Players[i]))
                {
                    PlayersBoard.SetCurrent(PlayersBoard.Players[i]);
                    break;
                }
            }
        }
        
        #region Serialization

        public void Serialize(PooledBitWriter writer)
        {
            writer.WriteInt32(ThemeIndex);
        }

        public void Deserialize(PooledBitReader reader)
        {
            ThemeIndex = reader.ReadInt32();
        }

        #endregion
        
        public override string ToString()
        {
            return $"[RemoveFinalRoundThemeCommand, ThemeIndex: {ThemeIndex}, Owner: {OwnerString}]";
        }
    }
}