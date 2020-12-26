using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Victorina.SI
{
    public class SiConverter
    {
#if UNITY_EDITOR
        [MenuItem("Victorina/Run Convert")]
        public static void RunConvertMenu()
        {
            Debug.Log("Run Convert");

            SiConverter converter = new SiConverter();
            converter.Convert();
        }
#endif

        public Package Convert()
        {
            XmlReader xmlReader = XmlReader.Create($"{Application.dataPath}/Data/content.xml");

            Package package = new Package();
            package.Rounds = ReadRounds(xmlReader);
            
            // foreach (Round round in package.Rounds)
            // {
            //     Debug.Log($"Round: {round.Name}");
            //     foreach (Theme theme in round.Themes)
            //     {
            //         Debug.Log($"Theme: {theme.Name}");
            //         Debug.Log($"Questions: {theme.Questions.Count}");
            //     }
            // }

            return package;
        }

        private List<Round> ReadRounds(XmlReader xmlReader)
        {
            List<Round> rounds = new List<Round>();
            xmlReader.ReadToFollowing("rounds");
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "round")
                {
                    Round round = ReadRound(xmlReader);
                    rounds.Add(round);
                }
                
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "rounds")
                    break;
            }
            return rounds;
        }
        
        private Round ReadRound(XmlReader xmlReader)
        {
            Round round = new Round();
            round.Name = xmlReader.GetAttribute("name");
            //Debug.Log($"Round:{round.Name}");
            round.Themes = ReadThemes(xmlReader);
            return round;
        }

        private List<Theme> ReadThemes(XmlReader xmlReader)
        {
            List<Theme> themes = new List<Theme>();

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "theme")
                {
                    Theme theme = ReadTheme(xmlReader);
                    themes.Add(theme);
                }
                
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "themes")
                    break;
            }

            return themes;
        }

        private Theme ReadTheme(XmlReader xmlReader)
        {
            Theme theme = new Theme();
            theme.Name = xmlReader.GetAttribute("name");;
            theme.Questions = ReadQuestions(xmlReader);
            return theme;
        }
        
        private List<Question> ReadQuestions(XmlReader xmlReader)
        {
            List<Question> questions = new List<Question>();
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "question")
                {
                    Question question = ReadQuestion(xmlReader);
                    questions.Add(question);
                    //Debug.Log($"Question: {question}, {xmlReader.Name}");
                }
                
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "questions")
                    break;
            }
            return questions;
        }

        private Question ReadQuestion(XmlReader xmlReader)
        {
            Question question = new Question();
            int price = int.Parse(xmlReader.GetAttribute("price"));
            question.Price = price;
            xmlReader.ReadToFollowing("scenario");
            xmlReader.ReadToFollowing("atom");
            question.Text = xmlReader.ReadInnerXml();
            xmlReader.ReadToFollowing("answer");
            question.Answer = xmlReader.ReadInnerXml();
            return question;
        }
    }
}