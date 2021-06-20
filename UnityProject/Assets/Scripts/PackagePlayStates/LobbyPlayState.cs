using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class LobbyPlayState : PackagePlayState
    {
        public override PlayStateType Type => PlayStateType.Lobby;
        public override void Serialize(PooledBitWriter writer) { }
        public override void Deserialize(PooledBitReader reader) { }
    }
}