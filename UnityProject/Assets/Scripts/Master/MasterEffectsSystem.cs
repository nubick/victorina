using Injection;

namespace Victorina
{
    public class MasterEffectsSystem
    {
        [Inject] private SendToPlayersService SendToPlayersService { get; set; } 
        [Inject] private PlayEffectsSystem PlayEffectsSystem { get; set; }
        
        public void PlaySound(int number)
        {
            SendToPlayersService.SendPlaySoundEffectCommand(number);
            PlayEffectsSystem.PlaySound(number);
        }
    }
}