using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

namespace Victorina
{
    public class PackageJsonConverter
    {
        private const string FileNameKey = "FileName";
        private const string StoryDotTypeKey = "StorDotType";
        private const string TextKey = "Text";
        private const string ImageKey = "Image";
        private const string AudioKey = "Audio";
        private const string VideoKey = "Video";
        private const string QuestionStoryKey = "QuestionStory";
        private const string AnswerStoryKey = "AnswerStory";
        
        #region ToJson
        
        public string ToJson(Package package)
        {
            JSONNode packageNode = new JSONObject();
            //packageNode.Add("Name", package.Name);
            packageNode.Add("Scheme", "Package");
            
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
            jsonNode.Add("Type", question.Type.ToString());
            jsonNode.Add("Scheme", "Question");
            jsonNode.Add("Id", question.Id);
            jsonNode.Add("Price", question.Price);

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

            return null;
        }

        #endregion
        
        #region Read

        public Package ReadPackage(string packageJson)
        {
            JSONNode packageNode = JSON.Parse(packageJson);

            //string name = packageNode["Name"];
            Package package = new Package();

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

            JSONArray themes = roundNode["Themes"].AsArray;
            foreach (JSONNode themeNode in themes)
            {
                Theme theme = ReadTheme(themeNode);
                round.Themes.Add(theme);
            }

            return round;
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
            question.Type = (QuestionType) Enum.Parse(typeof(QuestionType), questionNode["Type"]);
            question.Price = questionNode["Price"].AsInt;
            question.QuestionStory.AddRange(ReadStory(questionNode[QuestionStoryKey].AsArray));
            question.AnswerStory.AddRange(ReadStory(questionNode[AnswerStoryKey].AsArray));
            return question;
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