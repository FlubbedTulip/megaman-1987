using Pools;
using UnityEngine;

namespace Managers
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;   // For background music only

        //Play a one-shot SFX using the AudioSource pool
        public void PlaySound(AudioClip audioClip)
        {
            if (audioClip == null) return;

            // Get a pooled audio source
            var pooledAudioSource = AudioSourcePool.Instance.Get();
            pooledAudioSource.PlaySound(audioClip);
        }

        //Play/Stop background music
        public void PlayMusic(AudioClip musicClip, bool loop = true, float volume = 1f)
        {
            if (musicClip == null) return;

            musicSource.Stop();
            musicSource.loop = loop;
            musicSource.volume = volume;
            musicSource.clip = musicClip;
            musicSource.Play();
        }
        
        //Stop current music 
        public void StopMusic()
        {
            musicSource.Stop();
        }
    }
}