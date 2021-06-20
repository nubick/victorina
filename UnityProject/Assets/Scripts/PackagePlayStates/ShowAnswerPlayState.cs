using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class ShowAnswerPlayState : PackagePlayState
    {
        public NetRoundQuestion NetRoundQuestion { get; set; }
        
        public override PlayStateType Type => PlayStateType.ShowAnswer;
        
        public override void Serialize(PooledBitWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public override void Deserialize(PooledBitReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}