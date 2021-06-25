using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class NoRiskPlayState : PackagePlayState
    {
        public string QuestionId { get; set; }//don't sync, it needs only for Master
        
        public override PlayStateType Type => PlayStateType.NoRisk;
        
        public override void Serialize(PooledBitWriter writer) { }
        public override void Deserialize(PooledBitReader reader) { }
    }
}