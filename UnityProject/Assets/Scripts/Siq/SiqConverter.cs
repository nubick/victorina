using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Victorina
{
    public class SiqConverter
    {
        public Package Convert(string packageName)
        {
            Package package = new Package(packageName);
            XmlReader xmlReader = XmlReader.Create($"{Application.persistentDataPath}/{packageName}/content.xml");
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
                string path = $"{Application.persistentDataPath}/{package.Name}/Images/{question.ImagePath}";
                if (File.Exists(path))
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    question.ImageBytes = bytes;
                    Debug.Log($"Image bytes are loaded, size: {bytes.Length / 1024}kb");
                }
                else
                {
                    Debug.LogWarning($"Image doesn't exist by path: {path}");
                }
            }
        }
    }
}