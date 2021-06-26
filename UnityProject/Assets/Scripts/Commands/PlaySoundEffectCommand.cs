using Injection;
using MLAPI.Serialization.Pooled;

namespace Victorina.Commands
{
    public class PlaySoundEffectCommand : Command, IPlayerCommand, IServerCommand
    {
        [Inject] private PlayEffectsSystem PlayEffectsSystem { get; set; }
        
        public SoundId SoundId { get; set; }

        public override CommandType Type => CommandType.PlaySoundEffect;
        public bool CanSend() => true;
        public bool CanExecuteOnServer() => true;
        public void ExecuteOnClient() => Execute();
        public void ExecuteOnServer() => Execute();
        private void Execute() => PlayEffectsSystem.PlaySound(SoundId);
        public void Serialize(PooledBitWriter writer) => writer.WriteInt32((int) SoundId);
        public void Deserialize(PooledBitReader reader) => SoundId = (SoundId) reader.ReadInt32();
        public override string ToString() => $"[PlaySoundEffectCommand, SoundId: {SoundId}]";
    }
}