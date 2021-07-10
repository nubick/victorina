using System.Linq;
using Injection;
using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class RoundPlayState : PackagePlayState
    {
        [Inject] private PlayerFilesRepository PlayerFilesRepository { get; set; }
        
        public int RoundNumber { get; set; }
        public RoundType[] RoundTypes { get; set; }
        public string[] RoundNames { get; set; }
        public NetRound NetRound { get; set; }
        
        public string SelectedQuestionId { get; set; }//Don't sync, server only
        
        public override PlayStateType Type => PlayStateType.Round;
        
        public override void Serialize(PooledBitWriter writer)
        {
            writer.WriteInt32(RoundNumber);
            SerializationTools.SerializeEnumArray(writer, RoundTypes);
            SerializationTools.SerializeStringsArray(writer, RoundNames);
            DataSerializationService.SerializeNetRound(writer, NetRound);
        }

        public override void Deserialize(PooledBitReader reader)
        {
            RoundNumber = reader.ReadInt32();
            RoundTypes = SerializationTools.DeserializeEnumArray<RoundType>(reader);
            RoundNames = SerializationTools.DeserializeStringsArray(reader);
            NetRound = DataSerializationService.DeserializeNetRound(reader);
            UpdateIsDownloadedByMe(NetRound);
        }

        private void UpdateIsDownloadedByMe(NetRound netRound)
        {
            foreach (NetRoundQuestion roundQuestion in netRound.Themes.SelectMany(theme => theme.Questions))
                roundQuestion.IsDownloadedByMe = roundQuestion.FileIds.All(PlayerFilesRepository.IsDownloaded);
        }

        public override string ToString()
        {
            return $"[RoundPlayState, {nameof(RoundNumber)}: {RoundNumber}]";
        }
    }
}