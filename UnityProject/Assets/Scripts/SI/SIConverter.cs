using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Victorina
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
            //Дед 🎅🏼
            //string content = Resources.Load<TextAsset>("content").text;
            string content = Resources.Load<TextAsset>("Pack01/content").text;
            
            File.WriteAllText($"{Application.persistentDataPath}/content.xml", content);
            XmlReader xmlReader = XmlReader.Create($"{Application.persistentDataPath}/content.xml");

            Package package = new Package();
            package.Rounds = ReadRounds(xmlReader);
            
            LoadImages(package);
            
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
                    if(!IsEmpty(question))
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
            string type = xmlReader.GetAttribute("type");
            if (string.IsNullOrEmpty(type))
            {
                question.Text = xmlReader.ReadInnerXml();
            }
            else
            {
                if (type == "image")
                {
                    question.IsImage = true;
                    string fileName = xmlReader.ReadInnerXml();
                    //format = "@_august_243_2.jpg"
                    question.ImagePath = fileName.Substring(1);
                }
            }
            
            xmlReader.ReadToFollowing("answer");
            question.Answer = xmlReader.ReadInnerXml();
            return question;
        }

        private bool IsEmpty(Question question)
        {
            return question.Price == 0;
        }

        private void LoadImages(Package package)
        {
            List<Question> imageQuestions = package.Rounds.SelectMany(round => round.Themes.SelectMany(theme => theme.Questions.Where(question => question.IsImage))).ToList();
            Debug.Log($"Questions with image amount: {imageQuestions.Count}");

            foreach (Question question in imageQuestions)
            {
                string fileName = Path.GetFileNameWithoutExtension(question.ImagePath);
                string path = $"Pack01/Images/{fileName}";
                Sprite sprite = Resources.Load<Sprite>(path);
                
                if(sprite == null)
                    Debug.Log($"Sprite is null, path: {path}");
                
                question.Image = sprite;
            }
        }
    }
}