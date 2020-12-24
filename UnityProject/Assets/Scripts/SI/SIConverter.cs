using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Victorina.SI
{
    public class SiConverter
    {
        [MenuItem("Victorina/Run Convert")]
        public static void RunConvertMenu()
        {
            Debug.Log("Run Convert");

            SiConverter converter = new SiConverter();
            converter.Convert();
        }
        
        public void Convert()
        {
            string xml = ReadXmlFile();
            List<Question> questions = ReadQuestions(xml);
            Debug.Log($"Questions are loaded, amount: {questions.Count}");

            int number = 1;
            foreach (Question question in questions)
            {
                Debug.Log($"{number}:{question}");
                number++;
            }
        }

        private string ReadXmlFile()
        {
            string xml = File.ReadAllText($"{Application.dataPath}/Data/content.xml");
            Debug.Log($"Xml file is loaded, length: {xml.Length}");
            return xml;
        }

        private List<Question> ReadQuestions(string xml)
        {
            List<Question> questions = new List<Question>();

            XmlReader xmlReader = XmlReader.Create($"{Application.dataPath}/Data/content.xml");
            xmlReader.ReadToFollowing("question");
            do
            {
                int price = int.Parse(xmlReader.GetAttribute("price"));
                
                xmlReader.ReadToFollowing("atom");
                string questionText = xmlReader.ReadInnerXml();

                xmlReader.ReadToFollowing("answer");
                string questionAnswer = xmlReader.ReadInnerXml();

                Question question = new Question();
                question.Text = questionText;
                question.Answer = questionAnswer;
                question.Price = price;
                
                questions.Add(question);

            } while (xmlReader.ReadToFollowing("question"));
            return questions;
        }
    }
}