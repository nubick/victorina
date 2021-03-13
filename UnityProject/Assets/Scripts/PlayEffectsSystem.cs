using Assets.Scripts.Data;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class PlayEffectsSystem
    {
        [Inject] private PlayEffectsData Data { get; set; }
        [Inject] private AppState AppState { get; set; }
        
        public void Initialize()
        {
            AppState.Volume.SubscribeChanged(SetVolume);
            SetVolume(AppState.Volume.Value);
            
            MetagameEvents.PlaySoundEffectCommand.Subscribe(OnPlaySoundEffect);
        }

        private void SetVolume(float volume)
        {
            Data.SoundEffectsAudioSource.volume = volume;
        }

        public void PlaySound(int number)
        {
            AudioClip sound = GetAudioClip(number);
            if (sound != null)
            {
                Debug.Log($"Play sound effect: {sound.name}");
                Data.SoundEffectsAudioSource.PlayOneShot(sound);
            }
        }

        private AudioClip GetAudioClip(int number)
        {
            return number <= Data.AudioClips.Length ? Data.AudioClips[number - 1] : null;
        }

        private void OnPlaySoundEffect(SoundEffect soundEffect)
        {
            soundEffect.Play(Data.SoundEffectsAudioSource);
        }
    }
}