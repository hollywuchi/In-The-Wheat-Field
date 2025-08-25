using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void SetSound(SoundDetails sound)
    {
        audioSource.clip = sound.soundClip;
        audioSource.volume = sound.soundVolume;
        audioSource.pitch = Random.Range(sound.soundPitchMin, sound.soundPitchMax);
    }
}
