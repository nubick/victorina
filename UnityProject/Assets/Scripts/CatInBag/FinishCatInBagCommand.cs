using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class FinishCatInBagCommand : Command, IServerCommand
    {
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PlayStateData PlayStateData { get; set; }
        
        public override CommandType Type => CommandType.FinishCatInBag;
        private CatInBagPlayState PlayState => PlayStateData.As<CatInBagPlayState>();
        
        public bool CanExecuteOnServer()
        {
            if (PlayStateData.Type != PlayStateType.CatInBag)
            {
                Debug.Log($"Can't finish CatInBag in PlayState: {PlayStateData}");
                return false;
            }
            
            if (!PlayState.WasGiven)
            {
                Debug.Log("Can't finish CatInBag. It was not given.");
                return false;
            }
            
            return true;
        }

        public void ExecuteOnServer()
        {
            PlayStateSystem.ChangeToShowQuestionPlayState(PlayState.NetQuestion, PlayState.NetQuestion.CatInBagInfo.Price);
        }

        public override string ToString() => "[FinishCatInBagCommand]";
    }
}