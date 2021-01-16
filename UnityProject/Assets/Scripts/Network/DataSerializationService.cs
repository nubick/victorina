using System;
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
            SerializationManager.RegisterSerializationHandlers(SerializeStoryDot, DeserializeStoryDot);
            SerializationManager.RegisterSerializationHandlers(SerializeNetRoundsInfo, DeserializeNetRoundsInfo);
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
            writer.WriteString(netQuestion.Answer);
            writer.WriteInt32(netQuestion.StoryDotsAmount);
        }
        
        private NetQuestion DeserializeNetQuestion(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            NetQuestion netQuestion = new NetQuestion();
            netQuestion.Answer = reader.ReadString().ToString();
            netQuestion.StoryDotsAmount = reader.ReadInt32();
            return netQuestion;
        }

        private void SerializeStoryDot(Stream stream, StoryDot storyDot)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            if (storyDot is TextStoryDot textDot)
            {
                writer.WriteByte(0);
                writer.WriteString(textDot.Text);
            }
            else if (storyDot is ImageStoryDot imageDot)
            {
                writer.WriteByte(1);
                writer.WriteByteArray(imageDot.Bytes);
            }
        }

        private StoryDot DeserializeStoryDot(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            byte type = reader.ReadByteDirect();

            if (type == 0)
            {
                string text = reader.ReadString().ToString();
                TextStoryDot testDot = new TextStoryDot(text);
                return testDot;
            }
            
            if (type == 1)
            {
                byte[] bytes = reader.ReadByteArray();
                ImageStoryDot imageDot = new ImageStoryDot(bytes);
                return imageDot;
            }

            throw new Exception($"Not supported story dot type: {type}");
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