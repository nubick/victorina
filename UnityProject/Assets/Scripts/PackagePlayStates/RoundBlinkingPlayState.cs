using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class RoundBlinkingPlayState : PackagePlayState
    {
        public override PlayStateType Type => PlayStateType.RoundBlinking;
        
        public int RoundNumber { get; set; }
        public NetRoundQuestion NetRoundQuestion { get; set; }
        
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