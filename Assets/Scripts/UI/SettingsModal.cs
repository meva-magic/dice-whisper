using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsModal : Modal
{
    private const string SoundVolumeKey = "sound_volume";
    private const string MusicVolumeKey = "music_volume";

    [SerializeField] private Slider _soundVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;

    [SerializeField] private AudioMixer _audioMixer;

    private void Start()
    {
        bool firstLaunch = !PlayerPrefs.HasKey(SoundVolumeKey);

        if (firstLaunch)
        {
            _soundVolumeSlider.value = 1f;
            _musicVolumeSlider.value = 1f;

            _audioMixer.SetFloat(SoundVolumeKey, SliderValueToVolume(1f));
            _audioMixer.SetFloat(MusicVolumeKey, SliderValueToVolume(1f));
        }
        else
        {
            _soundVolumeSlider.value = PlayerPrefs.GetFloat(SoundVolumeKey);
            _musicVolumeSlider.value = PlayerPrefs.GetFloat(MusicVolumeKey);

            _audioMixer.SetFloat(SoundVolumeKey, SliderValueToVolume(_soundVolumeSlider.value));
            _audioMixer.SetFloat(MusicVolumeKey, SliderValueToVolume(_musicVolumeSlider.value));
        }
    }

    public void SetSoundVolume()
    {
        PlayerPrefs.SetFloat(SoundVolumeKey, _soundVolumeSlider.value);
        _audioMixer.SetFloat(SoundVolumeKey, SliderValueToVolume(_soundVolumeSlider.value));
    }

    public void SetMusicVolume()
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, _musicVolumeSlider.value);
        _audioMixer.SetFloat(MusicVolumeKey, SliderValueToVolume(_musicVolumeSlider.value));
    }

    private float SliderValueToVolume(float value)
    {
        // These values are in dB.
        float maxVolume = 0f;
        float minVolume = -80f;

        // Converting from a linear scale to a logarithmic scale in such a way
        // that the change in volume sounds subjectively close to linear.
        // https://stackoverflow.com/questions/46529147/how-to-set-a-mixers-volume-to-a-sliders-volume-in-unity
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * (maxVolume - minVolume) / 4f + maxVolume;
    }

    protected override void OnActivate() { }

    protected override void OnDeactivate()
        => PlayerPrefs.Save();
}
