using System.IO;
using MLAPI.Serialization;
using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public class DataSerializationService
    {
        public void Initialize()
        {
            SerializationManager.RegisterSerializationHandlers(SerializePlayersBoard, DeserializePlayersBoard);
            SerializationManager.RegisterSerializationHandlers(SerializeRoundData, DeserializeRoundData);
            SerializationManager.RegisterSerializationHandlers(SerializeNetRoundQuestion, DeserializeNetRoundQuestion);
            SerializationManager.RegisterSerializationHandlers(SerializeNetQuestion, DeserializeNetQuestion);
            SerializationManager.RegisterSerializationHandlers(SerializeNetRoundsInfo, DeserializeNetRoundsInfo);
            
            SerializationManager.RegisterSerializationHandlers(SerializeTextStoryDot, DeserializeTextStoryDot);
            SerializationManager.RegisterSerializationHandlers(SerializeImageStoryDot, DeserializeImageStoryDot);
            SerializationManager.RegisterSerializationHandlers(SerializeAudioStoryDot, DeserializeAudioStoryDot);
            SerializationManager.RegisterSerializationHandlers(SerializeVideoStoryDot, DeserializeVideoStoryDot);
        }

        #region PlayersBoard

        private void SerializePlayersBoard(Stream stream, PlayersBoard playersBoard)
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                writer.WriteInt32(playersBoard.PlayerNames.Count);
                foreach (string str in playersBoard.PlayerNames)
                    writer.WriteString(str);
            }
        }

        private PlayersBoard DeserializePlayersBoard(Stream stream)
        {
            PlayersBoard playersBoard = new PlayersBoard();
            using (PooledBitReader reader = PooledBitReader.Get(stream))
            {
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    string str = reader.ReadString().ToString();
                    playersBoard.PlayerNames.Add(str);
                }
            }
            return playersBoard;
        }

        #endregion
        
        #region RoundData

        private void SerializeRoundData(Stream stream, NetRound netRound)
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                writer.WriteInt32(netRound.Themes.Count);
                foreach (NetRoundTheme roundThemeData in netRound.Themes)
                    SerializeNetRoundTheme(writer, roundThemeData);
            }
        }

        private NetRound DeserializeRoundData(Stream stream)
        {
            NetRound netRound = new NetRound();
            using (PooledBitReader reader = PooledBitReader.Get(stream))
            {
                int amount = reader.ReadInt32();
                for (int i = 0; i < amount; i++)
                {
                    NetRoundTheme netRoundTheme = DeserializeRoundThemeData(reader);
                    netRound.Themes.Add(netRoundTheme);
                }
            }
            return netRound;
        }

        private void SerializeNetRoundTheme(PooledBitWriter writer, NetRoundTheme netRoundTheme)
        {
            writer.WriteString(netRoundTheme.Name);
            writer.WriteInt32(netRoundTheme.Questions.Count);
            foreach (NetRoundQuestion netRoundQuestion in netRoundTheme.Questions)
                SerializeNetRoundQuestion(writer, netRoundQuestion);
        }

        private NetRoundTheme DeserializeRoundThemeData(PooledBitReader reader)
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

        private void SerializeNetRoundQuestion(PooledBitWriter writer, NetRoundQuestion netRoundQuestion)
        {
            writer.WriteString(netRoundQuestion.QuestionId);
            writer.WriteInt32(netRoundQuestion.Price);
            writer.WriteBool(netRoundQuestion.IsAnswered);
        }
        
        private NetRoundQuestion DeserializeNetRoundQuestion(PooledBitReader reader)
        {
            string questionId = reader.ReadString().ToString();
            NetRoundQuestion netRoundQuestion = new NetRoundQuestion(questionId);
            netRoundQuestion.Price = reader.ReadInt32();
            netRoundQuestion.IsAnswered = reader.ReadBool();
            return netRoundQuestion;
        }
        
        #endregion

        #region NetQuestion
        
        private void SerializeNetQuestion(Stream stream, NetQuestion netQuestion)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32(netQuestion.QuestionStoryDotsAmount);
            writer.WriteInt32(netQuestion.AnswerStoryDotsAmount);
        }
        
        private NetQuestion DeserializeNetQuestion(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            NetQuestion netQuestion = new NetQuestion();
            netQuestion.QuestionStoryDotsAmount = reader.ReadInt32();
            netQuestion.AnswerStoryDotsAmount = reader.ReadInt32();
            return netQuestion;
        }

        #endregion
        
        #region Story Dots

        private void SerializeTextStoryDot(Stream stream, TextStoryDot textStoryDot)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32(textStoryDot.Index);
            writer.WriteString(textStoryDot.Text);
        }

        private TextStoryDot DeserializeTextStoryDot(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            int index = reader.ReadInt32();
            string text = reader.ReadString().ToString();
            return new TextStoryDot(index, text);
        }
        
        private void SerializeFileStoryDot(Stream stream, FileStoryDot fileStoryDot)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32(fileStoryDot.Index);
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
            fileStoryDot.Index = reader.ReadInt32();
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

        private void SerializeNetRoundsInfo(Stream stream, NetRoundsInfo netRoundsInfo)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32(netRoundsInfo.RoundsAmount);
            writer.WriteInt32(netRoundsInfo.CurrentRoundNumber);
        }

        private NetRoundsInfo DeserializeNetRoundsInfo(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            NetRoundsInfo netRoundsInfo = new NetRoundsInfo();
            netRoundsInfo.RoundsAmount = reader.ReadInt32();
            netRoundsInfo.CurrentRoundNumber = reader.ReadInt32();
            return netRoundsInfo;
        }
        
    }
}