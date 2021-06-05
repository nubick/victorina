using System.Collections.Generic;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class MasterContextKeyboardSystem : IKeyPressedHandler
    {
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }

        public List<ContextCommand> Commands { get; } = new List<ContextCommand>();
        
        public MasterContextKeyboardSystem()
        {
            RegisterCommands();    
        }
        
        private void RegisterCommands()
        {
            //Accepting Answer
            ContextCommand acceptAnswerAsCorrectCommand = new ContextCommand("Accept Answer As Correct");
            acceptAnswerAsCorrectCommand.Condition = () => MatchData.Phase.Value == MatchPhase.Question && 
                                                           QuestionAnswerData.Phase.Value == QuestionPhase.AcceptingAnswer;
            acceptAnswerAsCorrectCommand.KeyCode = KeyCode.LeftArrow;
            acceptAnswerAsCorrectCommand.Action = () => QuestionAnswerSystem.AcceptAnswerAsCorrect();
            acceptAnswerAsCorrectCommand.Tip = "Влево:\tВерно";
            Commands.Add(acceptAnswerAsCorrectCommand);

            ContextCommand acceptAnswerAsWrongCommand = new ContextCommand("Accept Answer as Wrong");
            acceptAnswerAsWrongCommand.Condition = () => MatchData.Phase.Value == MatchPhase.Question && 
                                                         QuestionAnswerData.Phase.Value == QuestionPhase.AcceptingAnswer;
            acceptAnswerAsWrongCommand.KeyCode = KeyCode.RightArrow;
            acceptAnswerAsWrongCommand.Action = () => QuestionAnswerSystem.AcceptAnswerAsWrong();
            acceptAnswerAsWrongCommand.Tip = "Вправо:\tНеверно";
            Commands.Add(acceptAnswerAsWrongCommand);

            ContextCommand cancelAcceptingAnswerCommand = new ContextCommand("Cancel Accepting Answer");
            cancelAcceptingAnswerCommand.Condition = () => MatchData.Phase.Value == MatchPhase.Question && 
                                                           QuestionAnswerData.Phase.Value == QuestionPhase.AcceptingAnswer;
            cancelAcceptingAnswerCommand.KeyCode = KeyCode.DownArrow;
            cancelAcceptingAnswerCommand.Action = () => QuestionAnswerSystem.CancelAcceptingAnswer();
            cancelAcceptingAnswerCommand.Tip = "Вниз:\t\tОтмена";
            Commands.Add(cancelAcceptingAnswerCommand);
            
            //Navigation
            ContextCommand showNextCommand = new ContextCommand("Show Next");
            showNextCommand.Condition = () => MatchData.Phase.Value == MatchPhase.Question &&
                                              QuestionAnswerData.Phase.Value == QuestionPhase.ShowQuestion &&
                                              QuestionAnswerSystem.CanShowNext();
            showNextCommand.KeyCode = KeyCode.Space;//todo: and KeyCode.RightArrow
            showNextCommand.Action = () => QuestionAnswerSystem.ShowNext();
            showNextCommand.Tip = "Пробел:\tДалее";
            Commands.Add(showNextCommand);
            
            ContextCommand showPreviousCommand = new ContextCommand("Show Previous");
            showPreviousCommand.Condition = () => MatchData.Phase.Value == MatchPhase.Question &&
                                                  QuestionAnswerData.Phase.Value == QuestionPhase.ShowQuestion &&
                                                  QuestionAnswerSystem.CanShowPrevious();
            showPreviousCommand.KeyCode = KeyCode.LeftArrow;
            showPreviousCommand.Action = () => QuestionAnswerSystem.ShowPrevious();
            showPreviousCommand.Tip = "Пробел:\tДалее";
            Commands.Add(showPreviousCommand);
            
            ContextCommand showAnswerCommand = new ContextCommand("Show Answer");
            showAnswerCommand.Condition = () => MatchData.Phase.Value == MatchPhase.Question &&
                                                QuestionAnswerData.Phase.Value == QuestionPhase.ShowQuestion &&
                                                QuestionAnswerSystem.CanShowAnswer() && 
                                                !QuestionAnswerData.PlayersButtonClickData.Players.Any();
            showAnswerCommand.KeyCode = KeyCode.Space;//todo: RightArrow
            showAnswerCommand.Action = () => QuestionAnswerSystem.ShowAnswer();
            showAnswerCommand.Tip = "Пробел:\tПоказать ответ";
            Commands.Add(showAnswerCommand);
            
            ContextCommand backToRoundCommand = new ContextCommand("Back to Round");
            backToRoundCommand.Condition = () => MatchData.Phase.Value == MatchPhase.Question &&
                                                 QuestionAnswerSystem.CanBackToRound();
            backToRoundCommand.KeyCode = KeyCode.Space;//todo: RightArrow
            backToRoundCommand.Action = () => QuestionAnswerSystem.BackToRound();
            backToRoundCommand.Tip = "Пробел:\tК раунду";
            Commands.Add(backToRoundCommand);
            
            //Who Answer Selection
            ContextCommand selectFastestPlayerCommand = new ContextCommand("Select the Fastest Player for Answer");
            selectFastestPlayerCommand.Condition = () => MatchData.Phase.Value == MatchPhase.Question &&
                                                         QuestionAnswerData.Phase.Value == QuestionPhase.ShowQuestion &&
                                                         QuestionAnswerSystem.CanShowAnswer() &&
                                                         QuestionAnswerData.PlayersButtonClickData.Players.Any();
            selectFastestPlayerCommand.KeyCode = KeyCode.Space;//todo: RightArrow
            selectFastestPlayerCommand.Action = () => QuestionAnswerSystem.SelectFastestPlayerForAnswer();
            selectFastestPlayerCommand.Tip = "Пробел:\tСамый быстрый";
            Commands.Add(selectFastestPlayerCommand);
        }
        
        public void OnKeyPressed(KeyCode keyCode)
        {
            if (NetworkData.IsClient)
                return;
            
            //Debug.Log($"Master keyboard: OnKeyPressed: {keyCode}");

            foreach (ContextCommand command in Commands)
            {
                if (command.Condition() && keyCode == command.KeyCode)
                {
                    Debug.Log($"Master keyboard: Execute command: '{command.Title}'");
                    command.Action();
                    return;
                }
            }
        }
    }
}