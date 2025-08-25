using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [Header("音乐数据库")]
    public SoundDetailList_SO soundDetailList;
    public SceneSoundList_SO sceneSoundList;

    [Header("Auido Source")]
    public AudioSource ambientSource;
    public AudioSource gameSource;

    private float MusicStartSecond => Random.Range(5, 10);
    private Coroutine soundRoutine;

    [Header("AudioMixer")]
    public AudioMixer audioMixer;

    [Header("Auido Snapshots")]
    public AudioMixerSnapshot normal;
    public AudioMixerSnapshot ambientOnly;
    public AudioMixerSnapshot mute;

    private float musicTransitionSecond = 4f;
    void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.PlaySoundEvent += OnPlaySoundEvent;
    }

    void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.PlaySoundEvent -= OnPlaySoundEvent;
    }



    private void OnAfterSceneLoadEvent()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        SceneSoundItem sceneSound = sceneSoundList.GetSceneSound(currentScene);
        if (sceneSound == null)
            return;

        SoundDetails ambient = soundDetailList.GetSoundDetails(sceneSound.ambient);
        SoundDetails gameSource = soundDetailList.GetSoundDetails(sceneSound.music);

        // PlayAmbientClip(ambient);
        // PlayMusicClip(gameSource);
        if (soundRoutine != null)
            StopCoroutine(soundRoutine);
        soundRoutine = StartCoroutine(PlaySoundRoutine(gameSource, ambient));

    }
    private void OnPlaySoundEvent(SoundName soundName)
    {
        var soundDetails = soundDetailList.GetSoundDetails(soundName);
        if (soundDetails != null)
            EventHandler.CallInitSoundEffect(soundDetails);
    }

    private IEnumerator PlaySoundRoutine(SoundDetails music, SoundDetails ambient)
    {
        if (music != null && ambient != null)
        {
            PlayAmbientClip(ambient, 1f);
            yield return new WaitForSeconds(MusicStartSecond);
            PlayMusicClip(music, musicTransitionSecond);
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayMusicClip(SoundDetails soundDetails, float musicSecond)
    {
        audioMixer.SetFloat("MusicVolume", ConvertSoundVolume(soundDetails.soundVolume));
        gameSource.clip = soundDetails.soundClip;
        if (gameSource.isActiveAndEnabled)
            gameSource.Play();

        normal.TransitionTo(musicSecond);
    }

    /// <summary>
    /// 播放环境音乐
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayAmbientClip(SoundDetails soundDetails, float musicSecond)
    {
        audioMixer.SetFloat("AmbientVolume", ConvertSoundVolume(soundDetails.soundVolume));
        ambientSource.clip = soundDetails.soundClip;
        if (ambientSource.isActiveAndEnabled)
            ambientSource.Play();

        ambientOnly.TransitionTo(musicSecond);
    }

    private float ConvertSoundVolume(float volume)
    {
        return volume * 100 - 80;
    }
}
