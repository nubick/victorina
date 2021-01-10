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
        
        private void SerializeTheme(PooledBitWriter writer, Theme theme)
        {
            writer.WriteString(theme.Id);
            writer.WriteString(theme.Name);
            writer.WriteInt32(theme.Questions.Count);
            foreach(Question question in theme.Questions)
                SerializeQuestion(writer, question);
        }

        private Theme DeserializeTheme(PooledBitReader reader)
        {
            string id = reader.ReadString().ToString();
            Theme theme = new Theme(id);
            theme.Name = reader.ReadString().ToString();
            int questionsAmount = reader.ReadInt32();
            for (int i = 0; i < questionsAmount; i++)
            {
                Question question = DeserializeQuestion(reader);
                theme.Questions.Add(question);
            }
            return theme;
        }

        private void SerializeQuestion(PooledBitWriter writer, Question question)
        {
            writer.WriteString(question.Id);
            writer.WriteInt32(question.Price);
            writer.WriteString(question.Text);
            writer.WriteString(question.Answer);
        }

        private Question DeserializeQuestion(PooledBitReader reader)
        {
            string id = reader.ReadString().ToString();
            Question question = new Question(id);
            question.Price = reader.ReadInt32();
            question.Text = reader.ReadString().ToString();
            question.Text = reader.ReadString().ToString();
            return question;
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
            writer.WriteString(netRoundQuestion.Text);
            writer.WriteString(netRoundQuestion.Answer);
            writer.WriteBool(netRoundQuestion.IsAnswered);
        }
        
        private NetRoundQuestion DeserializeNetRoundQuestion(PooledBitReader reader)
        {
            string questionId = reader.ReadString().ToString();
            NetRoundQuestion netRoundQuestion = new NetRoundQuestion(questionId);
            netRoundQuestion.Price = reader.ReadInt32();
            netRoundQuestion.Text = reader.ReadString().ToString();
            netRoundQuestion.Answer = reader.ReadString().ToString();
            netRoundQuestion.IsAnswered = reader.ReadBool();
            return netRoundQuestion;
        }
        
        #endregion

    }
}