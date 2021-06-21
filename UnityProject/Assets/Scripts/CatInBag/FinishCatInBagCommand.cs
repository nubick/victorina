using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class FinishCatInBagCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        
        public override CommandType Type => CommandType.FinishCatInBagCommand;
        
        public bool CanExecuteOnServer()
        {
            if (PlayStateData.Type != PlayStateType.CatInBag)
            {
                Debug.Log($"Can't finish CatInBag in PlayState: {PlayStateData}");
                return false;
            }

            CatInBagPlayState playState = PlayStateData.As<CatInBagPlayState>();
            if (!playState.WasGiven)
            {
                Debug.Log("Can't finish CatInBag. It was not given.");
                return false;
            }
            
            return true;
        }

        public void ExecuteOnServer()
        {
            ShowQuestionPlayState showQuestionPlayState = new ShowQuestionPlayState();
            PlayStateSystem.ChangePlayState(showQuestionPlayState);
        }
    }
}