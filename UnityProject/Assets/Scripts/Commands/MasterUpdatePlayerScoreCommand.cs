using UnityEngine;
using Victorina;
using Victorina.Commands;

namespace Commands
{
    public class MasterUpdatePlayerScoreCommand : Command, IServerCommand
    {
        public PlayerData Player { get; set; }
        public int NewScore { get; set; }
        
        public override CommandType Type => CommandType.MasterUpdatePlayerScore;
        
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