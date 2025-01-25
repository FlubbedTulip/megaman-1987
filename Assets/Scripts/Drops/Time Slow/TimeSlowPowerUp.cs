using System.Collections;
using UnityEngine;

public class TimeSlowPowerUp : MonoBehaviour
{
    [SerializeField] private float slowFactor = 0.2f;    // how slow the enemy becomes
    [SerializeField] private float duration = 3f;        // how long the effect lasts

    [Header("Audio Clips")]
    [SerializeField] private AudioClip slowMoSound;      // The SFX to play during slowdown



    private bool _isActive = false;
    private SpriteRenderer _renderer;



private void Awake() {
    _renderer = GetComponent<SpriteRenderer>();
}

private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player") && !_isActive)
    {
        _isActive = true;
        _renderer.enabled = false;
        StartCoroutine(SlowMotionRoutine());
    }
}

private IEnumerator SlowMotionRoutine()
    {
        // 1) Save old music state
        var musicSource = Managers.SoundManager.Instance.musicSource;
        bool wasMusicPlaying = musicSource.isPlaying;
        float oldMusicTime = musicSource.time; 

        // 2) Pause the music
        musicSource.Pause();

        // 3) Play the slowMoSound effect (one-shot or loop)
        // For a loop, you might spawn a pooled audio source or do it on SoundManager 
        // e.g. SoundManager.Instance.PlaySound(slowMoSound);
        // Or if you want it looping, you'd have an AudioSource for it, etc.
        Managers.SoundManager.Instance.PlaySound(slowMoSound);

        // 4) Apply time slowdown
        TimeSlower.SlowFactor = slowFactor;

        // 5) Wait "duration" real-time seconds
        // So it doesn't depend on game time
        yield return new WaitForSecondsRealtime(duration);

        // 6) Revert slowdown
        TimeSlower.SlowFactor = 1f;

        // 8) Resume music from where it left off
        if (wasMusicPlaying)
        {
            musicSource.time = oldMusicTime;  // optional if you want to resume at the exact point
            musicSource.Play();
        }

        Destroy(gameObject);
    }
}