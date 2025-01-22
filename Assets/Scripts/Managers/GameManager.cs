using Pools;
using UnityEngine;

namespace Managers
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [SerializeField] private AudioSource soundFXObject;

        public void PlaySound(AudioClip audioClip, Transform spawnTransform, float pitch)
        {
            // Get a pooled audio source
            var pooledAudioSource = AudioSourcePool.Instance.Get();
            pooledAudioSource.transform.position = spawnTransform.position;
            
            //play sound
            pooledAudioSource.PlaySound(audioClip, pitch);
        }
   
    }
}