using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class CrafterDragAndDropSystem
    {
        [Inject] private CrafterDragAndDropData Data { get; set; }
        [Inject] private CrafterData CrafterData { get; set; }
        [Inject] private PackageCrafterView PackageCrafterView { get; set; }
        
        public void Initialize()
        {
            MetagameEvents.CrafterQuestionBeginDrag.Subscribe(OnBeginDrag);
            MetagameEvents.CrafterQuestionEndDrag.Subscribe(OnEndDrag);
            MetagameEvents.CrafterQuestionDrop.Subscribe(OnQuestionDropped);
            MetagameEvents.CrafterQuestionDropOnQuestion.Subscribe(OnQuestionDropOnQuestion);
            MetagameEvents.CrafterQuestionDropOnTheme.Subscribe(OnQuestionDropOnTheme);
            MetagameEvents.CrafterQuestionDropOnRound.Subscribe(OnQuestionDropOnRound);
        }

        private void OnBeginDrag(CrafterQuestionDragItem questionItem)
        {
            questionItem.transform.SetParent(Data.DragArea);
        }

        private void OnEndDrag(CrafterQuestionDragItem questionItem)
        {
            if (questionItem.QuestionWidget == null)
                Object.Destroy(questionItem.gameObject);
            else
                questionItem.transform.SetParent(questionItem.QuestionWidget.transform, worldPositionStays: true);

            if (Data.WasChanges)
            {
                PackageCrafterView.RefreshUI();
                Data.WasChanges = false;
            }
        }

        private void OnQuestionDropped(CrafterQuestionDragItem questionItem)
        {
            if (CrafterData.SelectedTheme == null)
                return;

            Question question = questionItem.QuestionWidget.Question;

            Theme questionTheme = PackageTools.GetQuestionTheme(CrafterData.SelectedPackage, question);

            if (CrafterData.SelectedTheme != questionTheme)
            {
                Debug.Log($"Move question to theme: {CrafterData.SelectedTheme}");
                //questionTheme.Questions.Remove(question);
                //CrafterData.SelectedTheme.Questions.Add(question);
                //PackageFilesSystem.UpdatePackageJson(CrafterData.SelectedPackage);
            }
        }

        private void OnQuestionDropOnQuestion(CrafterQuestionDragItem droppedItem, CrafterQuestionDragItem targetItem)
        {
            Question droppedQuestion = droppedItem.QuestionWidget.Question;
            Question targetQuestion = targetItem.QuestionWidget.Question;
            
            Theme sourceTheme = PackageTools.GetQuestionTheme(CrafterData.SelectedPackage, droppedQuestion);
            Theme targetTheme = PackageTools.GetQuestionTheme(CrafterData.SelectedPackage, targetQuestion);

            sourceTheme.Questions.Remove(droppedQuestion);
            int insertIndex = targetTheme.Questions.IndexOf(targetQuestion);
            targetTheme.Questions.Insert(insertIndex, droppedQuestion);

            Data.WasChanges = true;
        }

        private void OnQuestionDropOnTheme(Question question, Theme theme)
        {
            Theme sourceTheme = PackageTools.GetQuestionTheme(CrafterData.SelectedPackage, question);
            sourceTheme.Questions.Remove(question);
            theme.Questions.Add(question);
            Data.WasChanges = true;
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
    }
}