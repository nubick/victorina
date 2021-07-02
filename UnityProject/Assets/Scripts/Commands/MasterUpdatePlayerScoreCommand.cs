using Injection;
using UnityEngine;
using Victorina;
using Victorina.Commands;

namespace Commands
{
    public class MasterUpdatePlayerScoreCommand : Command, IServerCommand
    {
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        
        public byte PlayerId { get; }
        public int NewScore { get; }
        
        public override CommandType Type => CommandType.MasterUpdatePlayerScore;
        private PlayerData Player => PlayersBoardSystem.GetPlayer(PlayerId);
        
        public MasterUpdatePlayerScoreCommand(byte playerId, int newScore)
        {
            PlayerId = playerId;
            NewScore = newScore;
        }
        
        public bool CanExecuteOnServer()
        {
            if (NewScore == Player.Score)
            {
                Debug.Log($"Cmd: Can't update score. Current player score the same: {Player.Score}");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            Debug.Log($"Update player score from '{Player.Score}' to '{NewScore}'");
            Player.Score = NewScore;
        }

        public override string ToString() => $"[MasterUpdatePlayerScoreCommand, Player: {Player}, NewScore: {NewScore}]";
    }
}