using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class FinalRoundPlayState : PackagePlayState
    {
        public override PlayStateType Type => PlayStateType.FinalRound;
        
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