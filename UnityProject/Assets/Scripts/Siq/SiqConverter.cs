using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class SiqConverter
    {
        [Inject] private EncodingFixSystem EncodingFixSystem { get; set; }
        
        public Package Convert(string packageName)
        {
            Package package = new Package(packageName);
            XmlReader xmlReader = XmlReader.Create($"{Static.DataPath}/{packageName}/content.xml");
            package.Rounds = ReadRounds(xmlReader);
            
            
            EncodingFixSystem.TryFix(packageName);
            
            LoadFiles(package);
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
            ReadScenario(xmlReader, question);
            
            xmlReader.ReadToFollowing("answer");
            question.Answer = xmlReader.ReadInnerXml();
            return question;
        }

        private void ReadScenario(XmlReader xmlReader, Question question)
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name == "atom")
                    {
                        ReadAtom(xmlReader, question);
                    }
                    else
                    {
                        throw new Exception($"Not supported element '{xmlReader.Name}'");
                    }
                }
                
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "scenario")
                    break;
            }
        }
        
        private void ReadAtom(XmlReader xmlReader, Question question)
        {
            string type = xmlReader.GetAttribute("type");
            xmlReader.Read();

            if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "atom")
                return;//<atom></atom> - skip
            
            if (xmlReader.NodeType != XmlNodeType.Text)
                throw new Exception($"Next NodeType is not text, it is '{xmlReader.NodeType}'");
            
            if (string.IsNullOrEmpty(type))
            {
                string text = xmlReader.Value;
                TextStoryDot textDot = new TextStoryDot(text);
                question.QuestionStory.Add(textDot);
            }
            else
            {
                if (type == "say")
                {
                    string text = xmlReader.Value;
                    TextStoryDot textDot = new TextStoryDot($"say: {text}");
                    question.QuestionStory.Add(textDot);
                }
                else if (type == "image")
                {
                    string fileName = xmlReader.Value; //format = "@_august_243_2.jpg"
                    ImageStoryDot imageStoryDot = new ImageStoryDot();
                    imageStoryDot.SiqPath = fileName.Substring(1);
                    question.QuestionStory.Add(imageStoryDot);
                }
                else if (type == "voice")
                {
                    string fileName = xmlReader.Value;
                    AudioStoryDot audioStoryDot = new AudioStoryDot();
                    audioStoryDot.SiqPath= fileName.Substring(1);
                    question.QuestionStory.Add(audioStoryDot);
                }
                else if (type == "video")
                {
                    string fileName = xmlReader.Value;
                    VideoStoryDot videoStoryDot = new VideoStoryDot();
                    videoStoryDot.SiqPath = fileName.Substring(1);
                    question.QuestionStory.Add(videoStoryDot);
                }
                else
                {
                    Debug.LogWarning($"Not supported atom type '{type}'");
                }
            }
        }

        private bool IsEmpty(Question question)
        {
            return question.Price == 0;
        }

        private void LoadFiles(Package package)
        {
            List<Question> allQuestions = package.Rounds.SelectMany(round => round.Themes.SelectMany(theme => theme.Questions)).ToList();
            foreach (Question question in allQuestions)
            {
                int index = 0;
                foreach (StoryDot storyDot in question.QuestionStory)
                {
                    storyDot.Index = index;
                    index++;
                    
                    if (storyDot is ImageStoryDot imageStoryDot)
                        imageStoryDot.SiqPath = $"{GetImagesPath(package.Name)}/{imageStoryDot.SiqPath}";
                    else if (storyDot is AudioStoryDot audioStoryDot)
                        audioStoryDot.SiqPath = $"{GetAudioPath(package.Name)}/{audioStoryDot.SiqPath}";
                    else if (storyDot is VideoStoryDot videoStoryDot)
                        videoStoryDot.SiqPath = $"{GetVideoPath(package.Name)}/{videoStoryDot.SiqPath}";
                }
            }
            Debug.Log("Files are loaded");
        }
        
        public string GetImagesPath(string packageName)
        {
            return $"{Static.DataPath}/{packageName}/Images";
        }
        
        public string GetAudioPath(string packageName)
        {
            return $"{Static.DataPath}/{packageName}/Audio";
        }

        public string GetVideoPath(string packageName)
        {
            return $"{Static.DataPath}/{packageName}/Video";
        }
    }
}