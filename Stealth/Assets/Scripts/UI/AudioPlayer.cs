using UnityEngine;
using UnityEngine.UI;

public class AudioPlayer : MonoBehaviour
{
    private const string soundVolumePref = "soundVolume";
    [SerializeField] private Slider soundVolume;

    [SerializeField] private AudioClip[] music;
    [SerializeField] private AudioSource source;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey(soundVolumePref))
            PlayerPrefs.SetFloat(soundVolumePref, 0.2f);
        source.volume = PlayerPrefs.GetFloat(soundVolumePref);
        if (soundVolume != null)
            soundVolume.value = source.volume;
    }

    private void Update()
    {
        if (music.Length > 0) {
            if (!source.isPlaying)
            {
                source.clip = music[Random.Range(0, music.Length)];
                source.Play();
            }
        }
        else Debug.Log("Music is not set in Audio Player!!");
    }

    public void changeVolume()
    {
        source.volume = soundVolume.value;
        PlayerPrefs.SetFloat(soundVolumePref, soundVolume.value);
    }
}
