using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class ShowQuestionPlayState : PackagePlayState
    {
        public override PlayStateType Type => PlayStateType.ShowQuestion;
        
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