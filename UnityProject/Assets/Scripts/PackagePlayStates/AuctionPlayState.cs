using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class AuctionPlayState : PackagePlayState
    {
        public override PlayStateType Type => PlayStateType.Auction;
        
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