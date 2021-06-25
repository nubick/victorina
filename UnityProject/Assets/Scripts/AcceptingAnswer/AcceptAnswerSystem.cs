using Injection;
using Victorina.Commands;

namespace Victorina
{
    public class AcceptAnswerSystem
    {
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        
        public void CancelAcceptingAnswer()
        {
            CommandsSystem.AddNewCommand(new CancelAcceptingAnswerCommand());
        }

        public void AcceptAnswerAsCorrect()
        {
            CommandsSystem.AddNewCommand(new AcceptAnswerAsCorrectCommand());
        }

        public void AcceptAnswerAsWrong()
        {
           CommandsSystem.AddNewCommand(new AcceptAnswerAsWrongCommand());
        }
    }
}