using System.Collections.Generic;
using UnityEngine;

namespace Victorina
{
    public class PackageProgress
    {
        private HashSet<string> AnsweredQuestionIds { get; } = new HashSet<string>();

        public void SetQuestionAsAnswered(string questionId)
        {
            if(AnsweredQuestionIds.Contains(questionId))
                Debug.LogWarning($"Question with id '{questionId}' was answered before.");
            
            AnsweredQuestionIds.Add(questionId);
        }

        public bool IsAnswered(string questionId)
        {
            return AnsweredQuestionIds.Contains(questionId);
        }
    }
}