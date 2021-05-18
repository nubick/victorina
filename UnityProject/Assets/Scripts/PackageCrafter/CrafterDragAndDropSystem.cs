using Injection;
using UnityEngine;

namespace Victorina
{
    public class CrafterDragAndDropSystem
    {
        [Inject] private CrafterDragAndDropData Data { get; set; }
        [Inject] private CrafterData CrafterData { get; set; }
        [Inject] private PackageFilesSystem PackageFilesSystem { get; set; }
        [Inject] private PackageCrafterView PackageCrafterView { get; set; }
        
        public void Initialize()
        {
            MetagameEvents.CrafterQuestionBeginDrag.Subscribe(OnBeginDrag);
            MetagameEvents.CrafterQuestionDrop.Subscribe(OnQuestionDropped);
            MetagameEvents.CrafterQuestionDropOnQuestion.Subscribe(OnQuestionDropOnQuestion);
            MetagameEvents.CrafterQuestionDropOnTheme.Subscribe(OnQuestionDropOnTheme);
        }

        private void OnBeginDrag(CrafterQuestionDragItem questionItem)
        {
            //questionItem.transform.SetParent(Data.DragArea);
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

            PackageCrafterView.RefreshUI();
        }

        private void OnQuestionDropOnTheme(Question question, Theme theme)
        {
            Theme sourceTheme = PackageTools.GetQuestionTheme(CrafterData.SelectedPackage, question);
            sourceTheme.Questions.Remove(question);
            theme.Questions.Add(question);
            PackageCrafterView.RefreshUI();
        }
    }
}