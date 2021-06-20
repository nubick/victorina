using System.IO;
using System.Linq;
using Assets.Scripts.Utils;
using Injection;
using MLAPI.Serialization;
using MLAPI.Serialization.Pooled;
using Victorina.Commands;

namespace Victorina
{
    public class DataSerializationService
    {
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private PackagePlayStateSystem PackagePlayStateSystem { get; set; }
        
        public void Initialize()
        {
            SerializationManager.RegisterSerializationHandlers(SerializePlayersBoard, DeserializePlayersBoard);
            SerializationManager.RegisterSerializationHandlers(SerializeNetRoundQuestion, DeserializeNetRoundQuestion);
            SerializationManager.RegisterSerializationHandlers(SerializeNetQuestion, DeserializeNetQuestion);
            SerializationManager.RegisterSerializationHandlers(SerializePlayersButtonClickData, DeserializePlayersButtonClickData);
            SerializationManager.RegisterSerializationHandlers(SerializeAuctionData, DeserializeAuctionData);
            SerializationManager.RegisterSerializationHandlers(SerializeFinalRoundData, DeserializeFinalRoundData);
            
            SerializationManager.RegisterSerializationHandlers(SerializeQuestionAnswerData, DeserializeQuestionAnswerData);
            SerializationManager.RegisterSerializationHandlers(SerializeQuestionStoryShowData, DeserializeQuestionStoryShowData);
            SerializationManager.RegisterSerializationHandlers(SerializeTextStoryDot, DeserializeTextStoryDot);
            SerializationManager.RegisterSerializationHandlers(SerializeImageStoryDot, DeserializeImageStoryDot);
            SerializationManager.RegisterSerializationHandlers(SerializeAudioStoryDot, DeserializeAudioStoryDot);
            SerializationManager.RegisterSerializationHandlers(SerializeVideoStoryDot, DeserializeVideoStoryDot);

            SerializationManager.RegisterSerializationHandlers(SerializePackagePlayStateData, DeserializePackagePlayStateData);
            
            SerializationManager.RegisterSerializationHandlers(SerializeCommandNetworkData, DeserializeCommandNetworkData);
            
            SerializationManager.RegisterSerializationHandlers(SerializeBytesArray, DeserializeBytesArray);
        }
        
        #region PlayersBoard

        private void SerializePlayersBoard(Stream stream, PlayersBoard playersBoard)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32(playersBoard.Players.Count);
            foreach (PlayerData player in playersBoard.Players)
            {
                writer.WriteByte(player.PlayerId);
                writer.WriteString(player.Name);
                writer.WriteBool(player.IsConnected);
                writer.WriteInt32(player.Score);
                writer.WriteByte(player.FilesLoadingPercentage);
            }

            bool isCurrentSelected = playersBoard.Current != null;
            writer.WriteBool(isCurrentSelected);
            if (isCurrentSelected)
                writer.WriteByte(playersBoard.Current.PlayerId);
        }

        private PlayersBoard DeserializePlayersBoard(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            PlayersBoard playersBoard = new PlayersBoard();
            int amount = reader.ReadInt32();
            for (int i = 0; i < amount; i++)
            {
                byte playerId = (byte) reader.ReadByte();
                PlayerData player = new PlayerData(playerId);
                player.Name = reader.ReadString().ToString();
                player.IsConnected = reader.ReadBool();
                player.Score = reader.ReadInt32();
                player.FilesLoadingPercentage = (byte) reader.ReadByte();
                playersBoard.Players.Add(player);
            }

            bool isCurrentSelected = reader.ReadBool();
            if (isCurrentSelected)
            {
                byte currentPlayerId = (byte) reader.ReadByte();
                PlayerData current = playersBoard.Players.Single(_ => _.PlayerId == currentPlayerId);
                playersBoard.SetCurrent(current);
            }
            
            return playersBoard;
        }

        #endregion
        
        #region RoundData

        public static void SerializeNetRound(PooledBitWriter writer, NetRound netRound)
        {
            writer.WriteInt32(netRound.Themes.Count);
            foreach (NetRoundTheme roundThemeData in netRound.Themes)
                SerializeNetRoundTheme(writer, roundThemeData);
        }

        public static NetRound DeserializeNetRound(PooledBitReader reader)
        {
            NetRound netRound = new NetRound();
            int amount = reader.ReadInt32();
            for (int i = 0; i < amount; i++)
            {
                NetRoundTheme netRoundTheme = DeserializeRoundThemeData(reader);
                netRound.Themes.Add(netRoundTheme);
            }
            return netRound;
        }

        private static void SerializeNetRoundTheme(PooledBitWriter writer, NetRoundTheme netRoundTheme)
        {
            writer.WriteString(netRoundTheme.Name);
            writer.WriteInt32(netRoundTheme.Questions.Count);
            foreach (NetRoundQuestion netRoundQuestion in netRoundTheme.Questions)
                SerializeNetRoundQuestion(writer, netRoundQuestion);
        }

        private static NetRoundTheme DeserializeRoundThemeData(PooledBitReader reader)
        {
            NetRoundTheme netRoundTheme = new NetRoundTheme();
            netRoundTheme.Name = reader.ReadString().ToString();
            int amount = reader.ReadInt32();
            for (int i = 0; i < amount; i++)
            {
                NetRoundQuestion netRoundQuestion = DeserializeNetRoundQuestion(reader);
                netRoundTheme.Questions.Add(netRoundQuestion);
            }
            return netRoundTheme;
        }
        
        #endregion
        
        #region NetRoundQuestion
        
        private void SerializeNetRoundQuestion(Stream stream, NetRoundQuestion netRoundQuestion)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            SerializeNetRoundQuestion(writer, netRoundQuestion);
        }
        
        private NetRoundQuestion DeserializeNetRoundQuestion(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            NetRoundQuestion netRoundQuestion = DeserializeNetRoundQuestion(reader);
            return netRoundQuestion;
        }

        private static void SerializeNetRoundQuestion(PooledBitWriter writer, NetRoundQuestion netRoundQuestion)
        {
            writer.WriteString(netRoundQuestion.QuestionId);
            writer.WriteInt32(netRoundQuestion.Price);
            writer.WriteBool(netRoundQuestion.IsAnswered);
            writer.WriteInt32((int) netRoundQuestion.Type);
            writer.WriteString(netRoundQuestion.Theme);
            writer.WriteBool(netRoundQuestion.IsDownloadedByAll);
            writer.WriteIntArray(netRoundQuestion.FileIds);
        }
        
        private static NetRoundQuestion DeserializeNetRoundQuestion(PooledBitReader reader)
        {
            string questionId = reader.ReadString().ToString();
            NetRoundQuestion netRoundQuestion = new NetRoundQuestion(questionId);
            netRoundQuestion.Price = reader.ReadInt32();
            netRoundQuestion.IsAnswered = reader.ReadBool();
            netRoundQuestion.Type = (QuestionType) reader.ReadInt32();
            netRoundQuestion.Theme = reader.ReadString().ToString();
            netRoundQuestion.IsDownloadedByAll = reader.ReadBool();
            netRoundQuestion.FileIds = reader.ReadIntArray();
            return netRoundQuestion;
        }
        
        #endregion

        #region NetQuestion
        
        private void SerializeNetQuestion(Stream stream, NetQuestion netQuestion)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32((int) netQuestion.Type);
            if (netQuestion.Type == QuestionType.CatInBag)
            {
                writer.WriteString(netQuestion.CatInBagInfo.Theme);
                writer.WriteInt32(netQuestion.CatInBagInfo.Price);
                writer.WriteBool(netQuestion.CatInBagInfo.CanGiveYourself);
            }
            writer.WriteInt32(netQuestion.QuestionStoryDotsAmount);
            writer.WriteInt32(netQuestion.AnswerStoryDotsAmount);
        }
        
        private NetQuestion DeserializeNetQuestion(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            NetQuestion netQuestion = new NetQuestion();
            netQuestion.Type = (QuestionType) reader.ReadInt32();
            if (netQuestion.Type == QuestionType.CatInBag)
            {
                string theme = reader.ReadString().ToString();
                int price = reader.ReadInt32();
                bool canGiveYourself = reader.ReadBool();
                netQuestion.CatInBagInfo = new CatInBagInfo(theme, price, canGiveYourself);
            }
            netQuestion.QuestionStoryDotsAmount = reader.ReadInt32();
            netQuestion.AnswerStoryDotsAmount = reader.ReadInt32();
            return netQuestion;
        }

        #endregion
        
        #region Story Dots

        private void SerializeTextStoryDot(Stream stream, TextStoryDot textStoryDot)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteString(textStoryDot.Text);
        }

        private TextStoryDot DeserializeTextStoryDot(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            string text = reader.ReadString().ToString();
            return new TextStoryDot(text);
        }
        
        private void SerializeFileStoryDot(Stream stream, FileStoryDot fileStoryDot)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32(fileStoryDot.FileId);
            writer.WriteInt32(fileStoryDot.ChunksAmount);
        }
        
        private void SerializeImageStoryDot(Stream stream, ImageStoryDot imageStoryDot)
        {
            SerializeFileStoryDot(stream, imageStoryDot);
        }

        private void SerializeAudioStoryDot(Stream stream, AudioStoryDot audioStoryDot)
        {
            SerializeFileStoryDot(stream, audioStoryDot);
        }

        private void SerializeVideoStoryDot(Stream stream, VideoStoryDot videoStoryDot)
        {
            SerializeFileStoryDot(stream, videoStoryDot);
        }
        
        private void DeserializeFileStoryDot(Stream stream, FileStoryDot fileStoryDot)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            fileStoryDot.FileId = reader.ReadInt32();
            fileStoryDot.ChunksAmount = reader.ReadInt32();
        }

        private ImageStoryDot DeserializeImageStoryDot(Stream stream)
        {
            ImageStoryDot imageStoryDot = new ImageStoryDot();
            DeserializeFileStoryDot(stream, imageStoryDot);
            return imageStoryDot;
        }

        private AudioStoryDot DeserializeAudioStoryDot(Stream stream)
        {
            AudioStoryDot audioStoryDot = new AudioStoryDot();
            DeserializeFileStoryDot(stream, audioStoryDot);
            return audioStoryDot;
        }

        private VideoStoryDot DeserializeVideoStoryDot(Stream stream)
        {
            VideoStoryDot videoStoryDot = new VideoStoryDot();
            DeserializeFileStoryDot(stream, videoStoryDot);
            return videoStoryDot;
        }
        
        #endregion
        
        private void SerializeQuestionAnswerData(Stream stream, QuestionAnswerData data)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);

            writer.WriteInt32((int) data.Phase.Value);
            writer.WriteInt32((int)data.MasterIntention);
            
            writer.WriteInt32((int) data.TimerState);
            writer.WriteSingle(data.TimerResetSeconds);
            writer.WriteSingle(data.TimerLeftSeconds);

            writer.WriteByte(data.AnsweringPlayerId);
            SerializeBytesArray(stream, data.WrongAnsweredIds.ToArray());
            SerializeBytesArray(stream, data.AdmittedPlayersIds.ToArray());
        }

        private QuestionAnswerData DeserializeQuestionAnswerData(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            QuestionAnswerData data = new QuestionAnswerData();

            data.Phase.Value = (QuestionPhase) reader.ReadInt32();
            data.MasterIntention = (MasterIntention) reader.ReadInt32();
            
            data.TimerState = (QuestionTimerState) reader.ReadInt32();
            data.TimerResetSeconds = reader.ReadSingle();
            data.TimerLeftSeconds = reader.ReadSingle();

            data.AnsweringPlayerId = (byte) reader.ReadByte();
            data.WrongAnsweredIds.AddRange(DeserializeBytesArray(stream));
            data.AdmittedPlayersIds.AddRange(DeserializeBytesArray(stream));

            return data;
        }

        private void SerializeQuestionStoryShowData(Stream stream, QuestionStoryShowData data)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32((int) data.State);
            writer.WriteInt32(data.CurrentStoryDotIndex);
        }

        private QuestionStoryShowData DeserializeQuestionStoryShowData(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            QuestionStoryShowData data = new QuestionStoryShowData();
            data.State = (QuestionStoryShowDataState) reader.ReadInt32();
            data.CurrentStoryDotIndex = reader.ReadInt32();
            return data;
        }
        
        
        private void SerializePlayersButtonClickData(Stream stream, PlayersButtonClickData data)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32(data.Players.Count);
            foreach (PlayerButtonClickData player in data.Players)
            {
                writer.WriteByte(player.PlayerId);
                writer.WriteString(player.Name);
                writer.WriteSingle(player.Time);
            }
        }

        private PlayersButtonClickData DeserializePlayersButtonClickData(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            PlayersButtonClickData data = new PlayersButtonClickData();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                PlayerButtonClickData player = new PlayerButtonClickData();
                player.PlayerId = (byte) reader.ReadByte();
                player.Name = reader.ReadString().ToString();
                player.Time = reader.ReadSingle();
                data.Players.Add(player);
            }
            return data;
        }
        
        private void WritePlayerData(PooledBitWriter writer, PlayerData playerData)
        {
            writer.WriteByte(playerData?.PlayerId ?? (byte) 0);
        }

        private PlayerData ReadPlayerData(PooledBitReader reader)
        {
            byte playerId = reader.ReadByteDirect();
            return playerId == 0 ? null : PlayersBoardSystem.GetPlayer(playerId);
        }

        private void SerializeAuctionData(Stream stream, AuctionData auctionData)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32(auctionData.Bet);
            writer.WriteBool(auctionData.IsAllIn);
            WritePlayerData(writer, auctionData.Player);
            WritePlayerData(writer, auctionData.BettingPlayer);
            byte[] playerIds = auctionData.PassedPlayers.Select(_ => _.PlayerId).ToArray();
            SerializeBytesArray(stream, playerIds);
        }

        private AuctionData DeserializeAuctionData(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            AuctionData data = new AuctionData();
            data.Bet = reader.ReadInt32();
            data.IsAllIn = reader.ReadBool();
            data.Player = ReadPlayerData(reader);
            data.BettingPlayer = ReadPlayerData(reader);
            byte[] playerIds = DeserializeBytesArray(stream);
            playerIds.ForEach(playerId => data.PassedPlayers.Add(PlayersBoardSystem.GetPlayer(playerId)));
            return data;
        }

        private void SerializeFinalRoundData(Stream stream, FinalRoundData data)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32((int) data.Phase);
            SerializationTools.SerializeStringsArray(writer, data.Themes);
            SerializationTools.SerializeBooleansArray(writer, data.RemovedThemes);
            SerializationTools.SerializeBooleansArray(writer, data.DoneBets);
            SerializationTools.SerializeBooleansArray(writer, data.DoneAnswers);
            writer.WriteString(data.AcceptingInfo);
        }

        private FinalRoundData DeserializeFinalRoundData(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            FinalRoundPhase phase = (FinalRoundPhase) reader.ReadInt32();
            string[] themes = SerializationTools.DeserializeStringsArray(reader);
            bool[] removedThemes = SerializationTools.DeserializeBooleanArray(reader);
            FinalRoundData finalRoundData = new FinalRoundData(phase, themes, removedThemes);
            finalRoundData.SetDoneBets(SerializationTools.DeserializeBooleanArray(reader));
            finalRoundData.SetDoneAnswers(SerializationTools.DeserializeBooleanArray(reader));
            finalRoundData.SetAcceptingInfo(reader.ReadString().ToString());
            return finalRoundData;
        }
        
        private void SerializeBytesArray(Stream stream, byte[] bytes)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32(bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        private byte[] DeserializeBytesArray(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            int length = reader.ReadInt32();
            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, length);
            return bytes;
        }
        
        #region Commands
        
        private void SerializeCommandNetworkData(Stream stream, CommandNetworkData data)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32((int) data.Command.Type);
            data.Command.Serialize(writer);
        }

        private CommandNetworkData DeserializeCommandNetworkData(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            CommandType commandType = (CommandType) reader.ReadInt32();
            INetworkCommand command = CommandsSystem.CreateNetworkCommand(commandType);
            command.Deserialize(reader);
            return new CommandNetworkData(command);
        }
        
        #endregion

        private void SerializePackagePlayStateData(Stream stream, PackagePlayStateData data)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32((int) data.PlayState.Type);
            data.PlayState.Serialize(writer);
        }

        private PackagePlayStateData DeserializePackagePlayStateData(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            PlayStateType playStateType = (PlayStateType) reader.ReadInt32();
            PackagePlayStateData data = new PackagePlayStateData();
            data.PlayState = PackagePlayStateSystem.Create(playStateType);
            data.PlayState.Deserialize(reader);
            return data;
        }
    }
}