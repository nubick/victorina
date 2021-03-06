using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class GiveCatInBagCommand : Command, IServerCommand, INetworkCommand
    {
        [Inject] private PlayStateData PlayStateData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }

        public byte ReceiverPlayerId { get; set; }
        
        public override CommandType Type => CommandType.GiveCatInBag;
        private CatInBagPlayState CatInBagPlayState => PlayStateData.As<CatInBagPlayState>();
        
        public bool CanSend()
        {
            return CanExecuteOnServer();
        }

        public bool CanExecuteOnServer()
        {
            if (PlayStateData.Type != PlayStateType.CatInBag)
            {
                Debug.Log("It is not CatInBagPlayState now!");
                return false;
            }

            if (CatInBagPlayState.WasGiven)
            {
                Debug.Log("Cat in bag was given");
                return false;
            }

            if (Owner == CommandOwner.Player && !PlayersBoardSystem.IsCurrentPlayer(OwnerPlayer))
            {
                Debug.Log($"Only current player can give cat in bag, not {OwnerPlayer}");
                return false;
            }
            
            bool canGiveToPlayer = CatInBagPlayState.NetQuestion.CatInBagInfo.CanGiveYourself || !PlayersBoardSystem.IsCurrentPlayer(ReceiverPlayerId);
            if (!canGiveToPlayer)
            {
                Debug.Log($"Can't give cat in bag to current player {ReceiverPlayerId}, from player {OwnerString} request");
                return false;
            }

            return true;
        }

        public void ExecuteOnServer()
        {
            Debug.Log($"Give cat in bag to {ReceiverPlayerId} from {OwnerString} request");
            PlayersBoardSystem.MakePlayerCurrent(ReceiverPlayerId);
            CatInBagPlayState.WasGiven = true;
            PlayStateData.MarkAsChanged();
            ServerEvents.PlaySoundEffect.Publish(SoundId.MeowAngry.ToString());
        }

        public void Serialize(PooledBitWriter writer)
        {
            writer.WriteByte(ReceiverPlayerId);
        }

        public void Deserialize(PooledBitReader reader)
        {
            ReceiverPlayerId = (byte) reader.ReadByte();
        }

        public override string ToString() => $"[GiveCatInBagCommand, {nameof(ReceiverPlayerId)}: {ReceiverPlayerId}]";
    }
}