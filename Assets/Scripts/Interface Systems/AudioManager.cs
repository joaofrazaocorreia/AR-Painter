using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("AudioSources")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource tapHoldSFXAudioSource;

    [Header("BG Music")]
    [SerializeField] private List<AudioClip> musicPlaylist;
    [SerializeField] private bool muteMusic = false;
    [SerializeField][Range(0f, 1f)] private float startingVolume = 1f;
    private int musicIndex;

    [Header("Player SFX")]
    [SerializeField] private AudioClip buttonPressSound;
    [SerializeField] private AudioClip tapActionSound;
    [SerializeField] private AudioClip holdActionSound;
    [SerializeField] private AudioClip holdActionCancelSound;

    [Header("Game SFX")]

    [SerializeField] private AudioClip successSFX;
    [SerializeField] private AudioClip gameTimerTickSound;
    [SerializeField] private AudioClip gameTimerLastTickSound;
    [SerializeField] private AudioClip colorTimerTickSound;
    [SerializeField] private AudioClip colorTimerLastTickSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;


    private void Start()
    {
        musicIndex = Random.Range(0, musicPlaylist.Count);
        musicAudioSource.volume = startingVolume;
    }

    /// <summary>
    /// Keeps the background music playlist going.
    /// </summary>
    private void Update()
    {
        CycleBGMusic();
    }

    /// <summary>
    /// Plays a given AudioClip on the music AudioSource.
    /// </summary>
    /// <param name="music">The music to play.</param>
    private void PlayBGMusic(AudioClip music)
    {
        musicAudioSource.clip = music;
        musicAudioSource.Play();
    }

    /// <summary>
    /// Plays a given AudioClip on the SFX AudioSource.
    /// </summary>
    /// <param name="sfx">The SFX to play.</param>
    private void PlaySFX(AudioClip sfx)
    {
        sfxAudioSource.PlayOneShot(sfx);
    }

    /// <summary>
    /// Cycles the music playlist when the AudioSource is not playing anything.
    /// </summary>
    private void CycleBGMusic()
    {
        if (musicAudioSource.volume > 0)
        {
            if (!musicAudioSource.isPlaying)
            {
                if (++musicIndex > musicPlaylist.Count - 1)
                    musicIndex = 0;

                PlayBGMusic(musicPlaylist[musicIndex]);
            }
        }

        else
            musicAudioSource.Stop();
    }

    public void FadeMusicToVolume(float volume)
    {
        if (!muteMusic)
            StartCoroutine(FadeMusic(volume, 2f));
    }

    private IEnumerator FadeMusic(float targetVolume, float speed = 1f)
    {
        while (musicAudioSource.volume != targetVolume)
        {
            float difference = targetVolume - musicAudioSource.volume;

            if (difference < 0)
                musicAudioSource.volume -= Mathf.Min(Time.fixedDeltaTime * speed / 10, -difference);
            else
                musicAudioSource.volume += Mathf.Min(Time.fixedDeltaTime * speed / 10, difference);

            yield return null;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicAudioSource.volume = volume;
    }

    public void PlayUIButtonSFX()
    {
        PlaySFX(buttonPressSound);
    }

    public void PlayTapActionSFX()
    {
        PlaySFX(tapActionSound);
    }

    public void ToggleTapHoldSFX(bool play, float duration = 1)
    {
        if (play && !tapHoldSFXAudioSource.isPlaying)
        {
            tapHoldSFXAudioSource.clip = holdActionSound;
            tapHoldSFXAudioSource.pitch = holdActionSound.length / duration;

            tapHoldSFXAudioSource.Play();
        }

        else if (!play)
        {
            tapHoldSFXAudioSource.Stop();
            tapHoldSFXAudioSource.pitch = 1f;
            PlaySFX(holdActionCancelSound);
        }
    }

    public void PlaySucessSFX()
    {
        PlaySFX(successSFX);
    }

    public void PlayGameTimerTickSFX(bool lastTick = false)
    {
        if (!lastTick) PlaySFX(gameTimerTickSound);

        else PlaySFX(gameTimerLastTickSound);
    }

    public void PlayColorTimerTickSFX(bool lastTick = false)
    {
        if (!lastTick) PlaySFX(colorTimerTickSound);

        else PlaySFX(colorTimerLastTickSound);
    }

    public void PlayWinSFX()
    {
        PlaySFX(winSound);
    }

    public void PlayLoseSFX()
    {
        PlaySFX(loseSound);
    }
}
