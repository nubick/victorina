using System;
using System.Linq;
using Assets.Scripts.Utils;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class CrafterDragAndDropSystem
    {
        [Inject] private CrafterDragAndDropData Data { get; set; }
        [Inject] private CrafterData CrafterData { get; set; }
        [Inject] private PackageCrafterView PackageCrafterView { get; set; }
        [Inject] private PackageFilesSystem PackageFilesSystem { get; set; }
        
        public void Initialize()
        {
            MetagameEvents.CrafterBeginDrag.Subscribe(OnBeginDrag);
            MetagameEvents.CrafterEndDrag.Subscribe(OnEndDrag);
            MetagameEvents.CrafterQuestionDropOnQuestion.Subscribe(OnQuestionDropOnQuestion);
            MetagameEvents.CrafterQuestionDropOnTheme.Subscribe(OnQuestionDropOnTheme);
            MetagameEvents.CrafterQuestionDropOnRound.Subscribe(OnQuestionDropOnRound);
            MetagameEvents.CrafterThemeDropOnTheme.Subscribe(OnThemDropOnTheme);
        }
        
        private void OnBeginDrag(DragItemBase dragItem)
        {
            dragItem.transform.SetParent(Data.DragArea);
        }

        private void OnEndDrag(DragItemBase dragItem)
        {
            // if (questionItem.QuestionWidget == null)
            // {
            //     Object.Destroy(questionItem.gameObject);
            // }
            // else
            {
                dragItem.transform.SetParent(dragItem.Container);
                (dragItem.transform as RectTransform).SetLeftTopRightBottom(0f, 0f, 0f, 0f);
            }

            if (Data.WasChanges)
            {
                PackageCrafterView.RefreshUI();
                Data.WasChanges = false;
            }
        }
        
        private void OnQuestionDropOnQuestion(Question droppedQuestion, Question targetQuestion)
        {
            Theme sourceTheme = PackageTools.GetQuestionTheme(CrafterData.SelectedPackage, droppedQuestion);
            Theme targetTheme = PackageTools.GetQuestionTheme(CrafterData.SelectedPackage, targetQuestion);

            sourceTheme.Questions.Remove(droppedQuestion);
            int insertIndex = targetTheme.Questions.IndexOf(targetQuestion);
            targetTheme.Questions.Insert(insertIndex, droppedQuestion);

            Data.WasChanges = true;
            PackageFilesSystem.UpdatePackageJson(CrafterData.SelectedPackage);
        }

        private void OnQuestionDropOnTheme(Question question, Theme theme)
        {
            Theme sourceTheme = PackageTools.GetQuestionTheme(CrafterData.SelectedPackage, question);
            sourceTheme.Questions.Remove(question);
            theme.Questions.Add(question);
            
            Data.WasChanges = true;
            PackageFilesSystem.UpdatePackageJson(CrafterData.SelectedPackage);
        }

        private void OnQuestionDropOnRound(Question question, Round newRound)
        {
            Round questionRound = PackageTools.GetQuestionRound(CrafterData.SelectedPackage, question);
            if (newRound != questionRound && newRound.Themes.Any())
            {
                PackageTools.DeleteQuestion(CrafterData.SelectedPackage, question);
                if (newRound.Themes.Any())
                {
                    newRound.Themes.Last().Questions.Add(question);
                    Data.WasChanges = true;
                }
            }
        }

        private void OnThemDropOnTheme(Theme dropTheme, Theme receiverTheme)
        {
            Round round = PackageTools.GetThemeRound(CrafterData.SelectedPackage, dropTheme);

            if (!round.Themes.Contains(dropTheme))
                throw new Exception($"Round '{round}' doesn't contain theme '{dropTheme}'");

            if (receiverTheme != null && !round.Themes.Contains(receiverTheme))
                throw new Exception($"Round '{round}' doesn't contain theme '{receiverTheme}'");

            round.Themes.Remove(dropTheme);

            if (receiverTheme == null)
            {
                round.Themes.Add(dropTheme);
            }
            else
            {
                int index = round.Themes.IndexOf(receiverTheme);
                round.Themes.Insert(index, dropTheme);
            }
            
            Data.WasChanges = true;
        }
    }
}