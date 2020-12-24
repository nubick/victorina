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
            SerializationManager.RegisterSerializationHandlers(SerializeMatchData, DeserializeMatchData);
            SerializationManager.RegisterSerializationHandlers(SerializeTextQuestion, DeserializeTextQuestion);
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

        #region MatchData

        private void SerializeMatchData(Stream stream, MatchData matchData)
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                writer.WriteInt32((int) matchData.Phase);
            }
        }

        private MatchData DeserializeMatchData(Stream stream)
        {
            MatchData matchData = new MatchData();
            using (PooledBitReader reader = PooledBitReader.Get(stream))
            {
                matchData.Phase = (MatchPhase) reader.ReadInt32();
            }
            return matchData;
        }

        #endregion
        
        #region TextQuestion

        private void SerializeTextQuestion(Stream stream, TextQuestion textQuestion)
        {
            using (PooledBitWriter writer = PooledBitWriter.Get(stream))
            {
                writer.WriteString(textQuestion.Question);
                writer.WriteString(textQuestion.Answer);
            }
        }

        private TextQuestion DeserializeTextQuestion(Stream stream)
        {
            TextQuestion textQuestion = new TextQuestion();
            using (PooledBitReader reader = PooledBitReader.Get(stream))
            {
                textQuestion.Question = reader.ReadString().ToString();
                textQuestion.Answer = reader.ReadString().ToString();
            }
            return textQuestion;
        }

        #endregion
    }
}