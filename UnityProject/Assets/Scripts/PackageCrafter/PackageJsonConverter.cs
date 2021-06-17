using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

namespace Victorina
{
    public class PackageJsonConverter
    {
        private const string FileNameKey = "FileName";
        private const string AuthorKey = "Author";
        private const string StoryDotTypeKey = "StorDotType";
        private const string PriceKey = "Price";
        private const string TextKey = "Text";
        private const string ImageKey = "Image";
        private const string AudioKey = "Audio";
        private const string VideoKey = "Video";
        private const string QuestionStoryKey = "QuestionStory";
        private const string AnswerStoryKey = "AnswerStory";
        private const string TypeKey = "Type";

        private const string CatInBagKey = "CatInBag";
        private const string ThemeKey = "Theme";
        private const string CanGiveYourselfKey = "CanGiveYourself";
        
        #region ToJson
        
        public string ToJson(Package package)
        {
            JSONNode packageNode = new JSONObject();
            packageNode.Add("Scheme", "Package");
            packageNode.Add(AuthorKey, package.Author);

            JSONArray roundsArray = new JSONArray();
            packageNode.Add("Rounds", roundsArray);
            
            foreach (Round round in package.Rounds)
            {
                JSONNode roundNode = ToJsonNode(round);
                roundsArray.Add(roundNode);
            }
            
            return packageNode.ToString(2);
        }
        
        private JSONNode ToJsonNode(Round round)
        {
            JSONNode roundNode = new JSONObject();
            roundNode.Add("Name", round.Name);
            roundNode.Add(TypeKey, round.Type.ToString());
            roundNode.Add("Scheme", "Round");
            
            JSONArray themesArray = new JSONArray();
            roundNode.Add("Themes", themesArray);
            
            foreach (Theme theme in round.Themes)
            {
                JSONNode themeNode = ToJsonNode(theme);
                themesArray.Add(themeNode);
            }
            
            return roundNode;
        }
        
        private JSONNode ToJsonNode(Theme theme)
        {
            JSONNode themeNode = new JSONObject();
            themeNode.Add("Name", theme.Name);
            themeNode.Add("Scheme", "Theme");
            themeNode.Add("Id", theme.Id);
            
            JSONArray questionsArray = new JSONArray();
            themeNode.Add("Questions", questionsArray);
            
            foreach (Question question in theme.Questions)
            {
                JSONNode questionNode = ToJsonNode(question);
                questionsArray.Add(questionNode);
            }

            return themeNode;
        }

        public string ToJson(Theme theme)
        {
            return ToJsonNode(theme).ToString(2);
        }
        
        private JSONNode ToJsonNode(Question question)
        {
            JSONNode jsonNode = new JSONObject();
            jsonNode.Add(TypeKey, question.Type.ToString());
            jsonNode.Add("Scheme", "Question");
            jsonNode.Add("Id", question.Id);
            jsonNode.Add(PriceKey, question.Price);

            if (question.Type == QuestionType.CatInBag)
            {
                CatInBagStoryDot catInBagStoryDot = question.QuestionStory.First() as CatInBagStoryDot;
                JSONNode catInBagNode = ToJsonNode(catInBagStoryDot);
                jsonNode.Add(CatInBagKey, catInBagNode);
            }
            
            JSONArray questionStoryArray = new JSONArray();
            jsonNode.Add(QuestionStoryKey, questionStoryArray);
            foreach (StoryDot storyDot in question.QuestionStory)
            {
                JSONNode storyDotNode = ToJsonNode(storyDot);
                if (storyDotNode != null)
                    questionStoryArray.Add(storyDotNode);
            }

            JSONArray answerStoryArray = new JSONArray();
            jsonNode.Add(AnswerStoryKey, answerStoryArray);
            foreach (StoryDot storyDot in question.AnswerStory)
            {
                JSONNode storyDotNode = ToJsonNode(storyDot);
                if (storyDotNode != null)
                    answerStoryArray.Add(storyDotNode);
            }

            return jsonNode;
        }

        private JSONNode ToJsonNode(CatInBagStoryDot catInBagStoryDot)
        {
            JSONNode node = new JSONObject();
            node.Add(ThemeKey, catInBagStoryDot.Theme);
            node.Add(PriceKey, catInBagStoryDot.Price);
            node.Add(CanGiveYourselfKey, catInBagStoryDot.CanGiveYourself);
            return node;
        }
        
        private JSONNode ToJsonNode(StoryDot storyDot)
        {
            if (storyDot is TextStoryDot textStoryDot)
            {
                JSONNode node = new JSONObject();
                node.Add(StoryDotTypeKey, TextKey);
                node.Add(TextKey, textStoryDot.Text);
                return node;
            }

            if (storyDot is ImageStoryDot imageStoryDot)
            {
                JSONNode node = new JSONObject();
                node.Add(StoryDotTypeKey, ImageKey);
                node.Add(FileNameKey, imageStoryDot.FileName);
                return node;
            }

            if (storyDot is AudioStoryDot audioStoryDot)
            {
                JSONNode node = new JSONObject();
                node.Add(StoryDotTypeKey, AudioKey);
                node.Add(FileNameKey, audioStoryDot.FileName);
                return node;
            }

            if (storyDot is VideoStoryDot videoStoryDot)
            {
                JSONNode node = new JSONObject();
                node.Add(StoryDotTypeKey, VideoKey);
                node.Add(FileNameKey, videoStoryDot.FileName);
                return node;
            }

            if (storyDot is NoRiskStoryDot || storyDot is CatInBagStoryDot)
            {
                return null;
            }
            
            Debug.LogWarning($"Not supported story dot json serialization: {storyDot}");
            
            return null;
        }

        #endregion
        
        #region Read

        public Package ReadPackage(string packageJson)
        {
            JSONNode packageNode = JSON.Parse(packageJson);
            
            Package package = new Package();

            package.Author = packageNode[AuthorKey];
            
            JSONArray rounds = packageNode["Rounds"].AsArray;
            foreach (JSONNode roundNode in rounds)
            {
                Round round = ReadRound(roundNode);
                package.Rounds.Add(round);
            }
            
            return package;
        }

        private Round ReadRound(JSONNode roundNode)
        {
            Round round = new Round();
            round.Name = roundNode["Name"];

            round.Type = RoundType.Simple;
            if (Enum.TryParse(roundNode[TypeKey], out RoundType parsedRoundType))
                round.Type = parsedRoundType;
            else
                Debug.Log($"Can't parse round type '{roundNode[TypeKey]}'. Use 'Simple' type as default");

            JSONArray themes = roundNode["Themes"].AsArray;
            foreach (JSONNode themeNode in themes)
            {
                Theme theme = ReadTheme(themeNode);
                round.Themes.Add(theme);
            }

            return round;
        }

        public Theme ReadTheme(string themeJson)
        {
            JSONNode themeNode = JSON.Parse(themeJson);
            return ReadTheme(themeNode);
        }
        
        private Theme ReadTheme(JSONNode themeNode)
        {
            string id = themeNode["Id"];
            Theme theme = new Theme(id);
            theme.Name = themeNode["Name"];

            JSONArray questions = themeNode["Questions"].AsArray;
            foreach (JSONNode questionNode in questions)
            {
                Question question = ReadQuestion(questionNode);
                theme.Questions.Add(question);
            }

            return theme;
        }

        private Question ReadQuestion(JSONNode questionNode)
        {
            string id = questionNode["Id"];
            Question question = new Question(id);
            question.Type = (QuestionType) Enum.Parse(typeof(QuestionType), questionNode[TypeKey]);
            question.Price = questionNode[PriceKey].AsInt;
            question.QuestionStory.AddRange(ReadStory(questionNode[QuestionStoryKey].AsArray));
            question.AnswerStory.AddRange(ReadStory(questionNode[AnswerStoryKey].AsArray));

            if (question.Type == QuestionType.CatInBag)
            {
                CatInBagStoryDot catInBagStoryDot = ReadCatInBag(questionNode[CatInBagKey]);
                question.QuestionStory.Insert(0, catInBagStoryDot);
            }
            else if (question.Type == QuestionType.NoRisk)
            {
                NoRiskStoryDot noRiskStoryDot = new NoRiskStoryDot();
                question.QuestionStory.Insert(0, noRiskStoryDot);
            }
            
            return question;
        }

        private CatInBagStoryDot ReadCatInBag(JSONNode catInBagNode)
        {
            string theme = catInBagNode[ThemeKey];
            int price = catInBagNode[PriceKey].AsInt;
            bool canGiveYourself = catInBagNode[CanGiveYourselfKey].AsBool;
            return new CatInBagStoryDot(theme, price, canGiveYourself);
        }
        
        private List<StoryDot> ReadStory(JSONArray storyArrayNode)
        {
            List<StoryDot> story = new List<StoryDot>();
            foreach (JSONNode storyDotNode in storyArrayNode)
                story.Add(ReadStoryDot(storyDotNode));
            return story;
        }

        private StoryDot ReadStoryDot(JSONNode storyDotNode)
        {
            string type = storyDotNode[StoryDotTypeKey];

            if (type == TextKey)
            {
                return new TextStoryDot(storyDotNode[TextKey]);
            }

            if (type == ImageKey)
            {
                return new ImageStoryDot {FileName = storyDotNode[FileNameKey]};
            }
            
            if (type == AudioKey)
            {
                return new AudioStoryDot {FileName = storyDotNode[FileNameKey]};
            }

            if (type == VideoKey)
            {
                return new VideoStoryDot {FileName = storyDotNode[FileNameKey]};
            }

            throw new Exception($"Not supported StoryDotType: '{type}'");
        }
        
        #endregion
    }
}