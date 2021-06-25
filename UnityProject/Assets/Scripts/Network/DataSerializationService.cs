using System;
using System.IO;
using System.Linq;
using Injection;
using MLAPI.Serialization;
using MLAPI.Serialization.Pooled;
using Victorina.Commands;

namespace Victorina
{
    public class DataSerializationService
    {
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private PackagePlayStateSystem PackagePlayStateSystem { get; set; }
        
        public void Initialize()
        {
            SerializationManager.RegisterSerializationHandlers(SerializePlayersBoard, DeserializePlayersBoard);
            SerializationManager.RegisterSerializationHandlers(SerializeNetRoundQuestion, DeserializeNetRoundQuestion);
            SerializationManager.RegisterSerializationHandlers(SerializeNetQuestion, DeserializeNetQuestion);
            SerializationManager.RegisterSerializationHandlers(SerializePlayersButtonClickData, DeserializePlayersButtonClickData);
            SerializationManager.RegisterSerializationHandlers(SerializeFinalRoundData, DeserializeFinalRoundData);
            SerializationManager.RegisterSerializationHandlers(SerializeAnswerTimerData, DeserializeAnswerTimerData);
            SerializationManager.RegisterSerializationHandlers(SerializePackagePlayStateData, DeserializePackagePlayStateData);
            SerializationManager.RegisterSerializationHandlers(SerializeCommandNetworkData, DeserializeCommandNetworkData);
            SerializationManager.RegisterSerializationHandlers(SerializationTools.SerializeBytesArray, SerializationTools.DeserializeBytesArray);
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

        public static void SerializeNetRoundQuestion(PooledBitWriter writer, NetRoundQuestion netRoundQuestion)
        {
            writer.WriteString(netRoundQuestion.QuestionId);
            writer.WriteInt32(netRoundQuestion.Price);
            writer.WriteBool(netRoundQuestion.IsAnswered);
            writer.WriteInt32((int) netRoundQuestion.Type);
            writer.WriteBool(netRoundQuestion.IsDownloadedByAll);
            writer.WriteIntArray(netRoundQuestion.FileIds);
        }
        
        public static NetRoundQuestion DeserializeNetRoundQuestion(PooledBitReader reader)
        {
            string questionId = reader.ReadString().ToString();
            NetRoundQuestion netRoundQuestion = new NetRoundQuestion(questionId);
            netRoundQuestion.Price = reader.ReadInt32();
            netRoundQuestion.IsAnswered = reader.ReadBool();
            netRoundQuestion.Type = (QuestionType) reader.ReadInt32();
            netRoundQuestion.IsDownloadedByAll = reader.ReadBool();
            netRoundQuestion.FileIds = reader.ReadIntArray();
            return netRoundQuestion;
        }
        
        #endregion

        #region NetQuestion
        
        private void SerializeNetQuestion(Stream stream, NetQuestion netQuestion)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            SerializeNetQuestion(writer, netQuestion);
        }

        public static void SerializeNetQuestion(PooledBitWriter writer, NetQuestion netQuestion)
        {
            writer.WriteString(netQuestion.QuestionId);
            writer.WriteInt32((int) netQuestion.Type);
            writer.WriteString(netQuestion.Theme);
            if (netQuestion.Type == QuestionType.CatInBag)
            {
                writer.WriteString(netQuestion.CatInBagInfo.Theme);
                writer.WriteInt32(netQuestion.CatInBagInfo.Price);
                writer.WriteBool(netQuestion.CatInBagInfo.CanGiveYourself);
            }
            writer.WriteInt32(netQuestion.QuestionStoryDotsAmount);
            writer.WriteInt32(netQuestion.AnswerStoryDotsAmount);
            
            SerializeStory(writer, netQuestion.QuestionStory);
            SerializeStory(writer, netQuestion.AnswerStory);
        }

        private static void SerializeStory(PooledBitWriter writer, StoryDot[] story)
        {
            writer.WriteInt32(story.Length);
            foreach (StoryDot storyDot in story)
            {
                writer.WriteInt32((int) storyDot.Type);
                if (storyDot is TextStoryDot textStoryDot)
                    SerializeTextStoryDot(writer, textStoryDot);
                else if (storyDot is ImageStoryDot imageStoryDot)
                    SerializeImageStoryDot(writer, imageStoryDot);
                else if (storyDot is AudioStoryDot audioStoryDot)
                    SerializeAudioStoryDot(writer, audioStoryDot);
                else if (storyDot is VideoStoryDot videoStoryDot)
                    SerializeVideoStoryDot(writer, videoStoryDot);
                else
                    throw new Exception($"Not supported story dot: {storyDot}");
            }
        }

        private NetQuestion DeserializeNetQuestion(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            return DeserializeNetQuestion(reader);
        }

        public static NetQuestion DeserializeNetQuestion(PooledBitReader reader)
        {
            NetQuestion netQuestion = new NetQuestion();
            netQuestion.QuestionId = reader.ReadString().ToString();
            netQuestion.Type = (QuestionType) reader.ReadInt32();
            netQuestion.Theme = reader.ReadString().ToString();
            if (netQuestion.Type == QuestionType.CatInBag)
            {
                string theme = reader.ReadString().ToString();
                int price = reader.ReadInt32();
                bool canGiveYourself = reader.ReadBool();
                netQuestion.CatInBagInfo = new CatInBagInfo(theme, price, canGiveYourself);
            }
            netQuestion.QuestionStoryDotsAmount = reader.ReadInt32();
            netQuestion.AnswerStoryDotsAmount = reader.ReadInt32();

            netQuestion.QuestionStory = DeserializeStory(reader);
            netQuestion.AnswerStory = DeserializeStory(reader);
            
            return netQuestion;
        }

        private static StoryDot[] DeserializeStory(PooledBitReader reader)
        {
            int length = reader.ReadInt32();
            StoryDot[] story = new StoryDot[length];
            for (int i = 0; i < length; i++)
            {
                StoryDotType type = (StoryDotType) reader.ReadInt32();
                story[i] = type switch
                {
                    StoryDotType.Text => DeserializeTextStoryDot(reader),
                    StoryDotType.Image => DeserializeImageStoryDot(reader),
                    StoryDotType.Audio => DeserializeAudioStoryDot(reader),
                    StoryDotType.Video => DeserializeVideoStoryDot(reader),
                    _ => throw new Exception($"Not supported StoryDotType: {type}")
                };
            }
            return story;
        }

        #endregion
        
        #region Story Dots

        private static void SerializeTextStoryDot(PooledBitWriter writer, TextStoryDot textStoryDot)
        {
            writer.WriteString(textStoryDot.Text);
        }

        private static TextStoryDot DeserializeTextStoryDot(PooledBitReader reader)
        {
            string text = reader.ReadString().ToString();
            return new TextStoryDot(text);
        }
        
        private static void SerializeFileStoryDot(PooledBitWriter writer, FileStoryDot fileStoryDot)
        {
            writer.WriteInt32(fileStoryDot.FileId);
            writer.WriteInt32(fileStoryDot.ChunksAmount);
        }
        
        private static void SerializeImageStoryDot(PooledBitWriter writer, ImageStoryDot imageStoryDot)
        {
            SerializeFileStoryDot(writer, imageStoryDot);
        }

        private static void SerializeAudioStoryDot(PooledBitWriter writer, AudioStoryDot audioStoryDot)
        {
            SerializeFileStoryDot(writer, audioStoryDot);
        }

        private static void SerializeVideoStoryDot(PooledBitWriter writer, VideoStoryDot videoStoryDot)
        {
            SerializeFileStoryDot(writer, videoStoryDot);
        }
        
        private static void DeserializeFileStoryDot(PooledBitReader reader, FileStoryDot fileStoryDot)
        {
            fileStoryDot.FileId = reader.ReadInt32();
            fileStoryDot.ChunksAmount = reader.ReadInt32();
        }

        private static ImageStoryDot DeserializeImageStoryDot(PooledBitReader reader)
        {
            ImageStoryDot imageStoryDot = new ImageStoryDot();
            DeserializeFileStoryDot(reader, imageStoryDot);
            return imageStoryDot;
        }

        private static AudioStoryDot DeserializeAudioStoryDot(PooledBitReader reader)
        {
            AudioStoryDot audioStoryDot = new AudioStoryDot();
            DeserializeFileStoryDot(reader, audioStoryDot);
            return audioStoryDot;
        }

        private static VideoStoryDot DeserializeVideoStoryDot(PooledBitReader reader)
        {
            VideoStoryDot videoStoryDot = new VideoStoryDot();
            DeserializeFileStoryDot(reader, videoStoryDot);
            return videoStoryDot;
        }
        
        #endregion
        
        private void SerializeAnswerTimerData(Stream stream, AnswerTimerData data)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32((int) data.TimerState);
            writer.WriteSingle(data.TimerResetSeconds);
            writer.WriteSingle(data.TimerLeftSeconds);
        }

        private AnswerTimerData DeserializeAnswerTimerData(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            AnswerTimerData data = new AnswerTimerData();
            data.TimerState = (QuestionTimerState) reader.ReadInt32();
            data.TimerResetSeconds = reader.ReadSingle();
            data.TimerLeftSeconds = reader.ReadSingle();
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