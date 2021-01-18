using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            XmlReader xmlReader = XmlReader.Create($"{Application.persistentDataPath}/{packageName}/content.xml");
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
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "atom")
                {
                    if (xmlReader.Name == "atom")
                    {
                        string type = xmlReader.GetAttribute("type");
                        if (string.IsNullOrEmpty(type))
                        {
                            string text = xmlReader.ReadInnerXml();
                            TextStoryDot textDot = new TextStoryDot(text);
                            question.QuestionStory.Add(textDot);
                        }
                        else
                        {
                            if (type == "say")
                            {
                                string text = xmlReader.ReadInnerXml();
                                TextStoryDot textDot = new TextStoryDot($"say: {text}");
                                question.QuestionStory.Add(textDot);
                            }
                            else if (type == "image")
                            {
                                string fileName = xmlReader.ReadInnerXml();//format = "@_august_243_2.jpg"
                                string imagePath = fileName.Substring(1);
                                ImageStoryDot imageDot = new ImageStoryDot(imagePath);
                                question.QuestionStory.Add(imageDot);
                            }
                            else if (type == "voice")
                            {
                                string fileName = xmlReader.ReadInnerXml();
                                string audioPath = fileName.Substring(1);
                                AudioStoryDot imageDot = new AudioStoryDot(audioPath);
                                question.QuestionStory.Add(imageDot);
                            }
                            else if (type == "video")
                            {
                                string fileName = xmlReader.ReadInnerXml();
                                string videoPath = fileName.Substring(1);
                                VideoStoryDot videoDot = new VideoStoryDot(videoPath);
                                question.QuestionStory.Add(videoDot);
                            }
                            else
                            {
                                Debug.LogWarning($"Not supported atom type '{type}'");
                            }
                        }
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
                foreach (StoryDot dot in question.QuestionStory)
                {
                    dot.Index = index;
                    index++;

                    if (dot is ImageStoryDot imageStoryDot)
                        LoadImage(imageStoryDot, package.Name);
                    else if (dot is AudioStoryDot audioStoryDot)
                        LoadAudio(audioStoryDot, package.Name);
                    else if (dot is VideoStoryDot videoStoryDot)
                        LoadVideo(videoStoryDot, package.Name);
                }
            }
            Debug.Log("Files are loaded");
        }

        private void LoadImage(ImageStoryDot imageDot, string packageName)
        {
            string path = $"{GetImagesPath(packageName)}/{imageDot.Path}";
            imageDot.Bytes = LoadFile(path);
        }

        private void LoadAudio(AudioStoryDot audioStoryDot, string packageName)
        {
            string path = $"{GetAudioPath(packageName)}/{audioStoryDot.Path}";
            audioStoryDot.FullPath = path;
            audioStoryDot.Bytes = LoadFile(path);
        }

        private void LoadVideo(VideoStoryDot videoStoryDot, string packageName)
        {
            string path = $"{GetVideoPath(packageName)}/{videoStoryDot.Path}";
            videoStoryDot.Bytes = LoadFile(path);
        }
        
        private string GetPath(string packageName, string folder, string path)
        {
            return $"{Application.persistentDataPath}/{packageName}/{folder}/{path}";
        }

        private byte[] LoadFile(string path)
        {
            byte[] bytes = null;
            if (File.Exists(path))
            {
                bytes = File.ReadAllBytes(path);
                Debug.Log($"File bytes are loaded, size: {bytes.Length / 1024}kb");
            }
            else
            {
                Debug.LogWarning($"File doesn't exist by path: {path}");
            }
            return bytes;
        }

        public string GetImagesPath(string packageName)
        {
            return $"{Application.persistentDataPath}/{packageName}/Images";
        }
        
        public string GetAudioPath(string packageName)
        {
            return $"{Application.persistentDataPath}/{packageName}/Audio";
        }

        public string GetVideoPath(string packageName)
        {
            return $"{Application.persistentDataPath}/{packageName}/Video";
        }
    }
}