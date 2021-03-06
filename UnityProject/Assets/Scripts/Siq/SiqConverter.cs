using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace Victorina
{
    public class SiqConverter
    {
        public bool IsSiqFormat(string packagePath)
        {
            string path = $"{packagePath}/content.xml";
            return File.Exists(path);
        }
        
        public Package Convert(string siqPackagePath)
        {
            Package package = new Package();
            package.Path = siqPackagePath;
            XmlReader xmlReader = XmlReader.Create($"{siqPackagePath}/content.xml");
            package.Author = ReadAuthor(xmlReader);
            package.Rounds = ReadRounds(xmlReader);
            InitializeStoryDots(package, siqPackagePath);
            return package;
        }

        private string ReadAuthor(XmlReader xmlReader)
        {
            xmlReader.ReadToFollowing("author");
            return xmlReader.ReadElementContentAsString();
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
            round.Type = ReadRoundType(xmlReader);
            round.Themes = ReadThemes(xmlReader);
            return round;
        }

        private RoundType ReadRoundType(XmlReader xmlReader)
        {
            RoundType roundType = RoundType.Simple;
            string roundTypeValue = xmlReader.GetAttribute("type");
            if (!string.IsNullOrEmpty(roundTypeValue))
            {
                if (roundTypeValue == "final")
                    roundType = RoundType.Final;
                else
                    Debug.LogWarning($"Not supported round type: {roundTypeValue}");
            }
            return roundType;
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
                    //if(!IsEmpty(question))
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

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name == "type")
                    {
                        string typeName = xmlReader.GetAttribute("name");
                        if (typeName == "sponsored")
                        {
                            question.Type = QuestionType.NoRisk;
                        }
                        else if (typeName == "cat" || typeName == "bagcat")
                        {
                            ReadCatInBag(xmlReader, question);
                        }
                        else if (typeName == "auction")
                        {
                            question.Type = QuestionType.Auction;
                        }
                        else
                        {
                            Debug.LogWarning($"Not supported question type name: {typeName}");
                        }
            
                        xmlReader.ReadToFollowing("scenario");
                        break;
                    }
                    else if (xmlReader.Name == "scenario")
                        break;
                    else
                        Debug.LogWarning($"Not supported question element Name: {xmlReader.Name}");
                }
            }
            
            ReadScenario(xmlReader, question);
            
            xmlReader.ReadToFollowing("answer");
            string answer = DecodeString(xmlReader.ReadInnerXml());
            TextStoryDot textStoryDot = new TextStoryDot(answer);
            question.AnswerStory.Add(textStoryDot);
            return question;
        }

        private void ReadCatInBag(XmlReader xmlReader, Question question)
        {
            //Example 1:
            //<type name="cat"/>
            
            //Example 2:
            //<type name="cat">
            //  <param name="theme">Биология</param>
            //  <param name="cost">1000</param>
            //</type>
            
            string theme = string.Empty;
            int catPrice = 0;
            bool canGiveYourself = false;
            
            if (!xmlReader.IsEmptyElement)
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "type")
                        break;

                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "param")
                    {
                        string paramName = xmlReader.GetAttribute("name");
                        if (paramName == "theme")
                        {
                            xmlReader.Read();
                            theme = xmlReader.Value;
                        }
                        else if (paramName == "cost")
                        {
                            xmlReader.Read();
                            string priceString = xmlReader.Value;
                            if (!int.TryParse(priceString, out catPrice))
                            {
                                catPrice = 100;
                                Debug.LogWarning($"Can't parse cat in bag cost '{priceString}', use default value {catPrice}");
                            }
                        }
                        else if (paramName == "self")
                        {
                            xmlReader.Read();
                            string boolString = xmlReader.Value;
                            canGiveYourself = boolString == "true";
                        }
                        else if (paramName == "knows")
                        {
                            //<param name="knows">after</param>
                            //I ignore this settings
                        }
                        else
                        {
                            Debug.LogWarning($"Not supported cat param: {paramName}");
                        }
                    }
                }
            }
            
            //Debug.Log($"Cat in bag, theme: {theme}, price: {catPrice}");
            question.Type = QuestionType.CatInBag;
            if (catPrice == 0)
                catPrice = question.Price;
            question.CatInBagInfo = new CatInBagInfo(theme, catPrice, canGiveYourself);
        }
        
        private void ReadScenario(XmlReader xmlReader, Question question)
        {
            bool isAfterMarker = false;
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name == "atom")
                    {
                        ReadAtom(xmlReader, question, ref isAfterMarker);
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

        private void ReadAtom(XmlReader xmlReader, Question question, ref bool isAfterMarker)
        {
            string type = xmlReader.GetAttribute("type");
            xmlReader.Read();
            
            if (type == "marker")
            {
                isAfterMarker = true;
                return;
            }

            if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "atom")
                return; //<atom></atom> - skip

            if (xmlReader.NodeType != XmlNodeType.Text)
                throw new Exception($"Next NodeType is not text, it is '{xmlReader.NodeType}'");

            if (string.IsNullOrEmpty(type))
            {
                string text = DecodeString(xmlReader.Value);
                TextStoryDot textStoryDot = new TextStoryDot(text);
                AddStoryDot(question, textStoryDot, isAfterMarker);
            }
            else if (type == "say")
            {
                string text = DecodeString(xmlReader.Value);
                TextStoryDot textStoryDot = new TextStoryDot($"say: {text}");
                AddStoryDot(question, textStoryDot, isAfterMarker);
            }
            else if (type == "image")
            {
                string fileName = xmlReader.Value; //format = "@_august_243_2.jpg"
                ImageStoryDot imageStoryDot = new ImageStoryDot();
                imageStoryDot.FileName = fileName.Substring(1);
                AddStoryDot(question, imageStoryDot, isAfterMarker);
            }
            else if (type == "voice")
            {
                string fileName = xmlReader.Value;
                AudioStoryDot audioStoryDot = new AudioStoryDot();
                audioStoryDot.FileName = fileName.Substring(1);
                AddStoryDot(question, audioStoryDot, isAfterMarker);
            }
            else if (type == "video")
            {
                string fileName = xmlReader.Value;
                VideoStoryDot videoStoryDot = new VideoStoryDot();
                videoStoryDot.FileName = fileName.Substring(1);
                AddStoryDot(question, videoStoryDot, isAfterMarker);
            }
            else
            {
                Debug.LogWarning($"Not supported atom type '{type}'");
            }
        }

        private void AddStoryDot(Question question, StoryDot storyDot, bool isAfterMarker)
        {
            if(isAfterMarker)
                question.AnswerStory.Add(storyDot);
            else
                question.QuestionStory.Add(storyDot);
        }

        private bool IsEmpty(Question question)
        {
            return question.Price == 0;
        }

        private void InitializeStoryDots(Package package, string packagePath)
        {
            List<Theme> allThemes = package.Rounds.SelectMany(round => round.Themes).ToList();
            foreach (Theme theme in allThemes)
            {
                foreach (Question question in theme.Questions)
                {
                    InitializeStory(question.QuestionStory, packagePath, question, theme);
                    InitializeStory(question.AnswerStory, packagePath, question, theme);
                }
            }
            Debug.Log($"Package '{package.Path}' files are loaded");
        }

        private void InitializeStory(List<StoryDot> story, string packagePath, Question question, Theme theme)
        {
            foreach (StoryDot storyDot in story)
            {
                if (storyDot is ImageStoryDot imageStoryDot)
                    imageStoryDot.Path = $"{GetImagesPath(packagePath)}/{imageStoryDot.FileName}";
                else if (storyDot is AudioStoryDot audioStoryDot)
                    audioStoryDot.Path = $"{GetAudioPath(packagePath)}/{audioStoryDot.FileName}";
                else if (storyDot is VideoStoryDot videoStoryDot)
                    videoStoryDot.Path = $"{GetVideoPath(packagePath)}/{videoStoryDot.FileName}";
            }

            if (question.Type == QuestionType.CatInBag)
            {
                if (question.CatInBagInfo.Price == 0)
                    question.CatInBagInfo.Price = question.Price;

                if (string.IsNullOrEmpty(question.CatInBagInfo.Theme))
                    question.CatInBagInfo.Theme = theme.Name;
            }
        }

        public string GetImagesPath(string packagePath)
        {
            return $"{packagePath}/Images";
        }
        
        public string GetAudioPath(string packagePath)
        {
            return $"{packagePath}/Audio";
        }

        public string GetVideoPath(string packagePath)
        {
            return $"{packagePath}/Video";
        }

        private void Log(XmlReader xmlReader)
        {
            Debug.Log($"NodeType: {xmlReader.NodeType}, Name: {xmlReader.Name}, ValueType: {xmlReader.ValueType}, Value: {xmlReader.Value}");
        }
        
        private string DecodeString(string str)
        {
            return str
                .Replace("&amp;", "&")
                .Replace("&gt;", ">")
                .Replace("&lt;", "<")
                .Replace("&quot;", "\"")
                .Replace("&apos;", "'");
        }
    }
}