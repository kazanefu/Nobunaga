using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Serializable]
    private class AudioEntry
    {
        public string key;
        public AudioClip clip;
    }

    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip defaultBgm;
    [SerializeField] private bool playDefaultBgmOnAwake = true;
    [SerializeField] private AudioEntry[] bgmClips = Array.Empty<AudioEntry>();
    [SerializeField] private AudioEntry[] seClips = Array.Empty<AudioEntry>();

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float initialBgmVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float initialSeVolume = 1f;
    [SerializeField] private bool saveVolumeSettings = true;

    private const string BgmVolumeKey = "AudioManager.BgmVolume";
    private const string SeVolumeKey = "AudioManager.SeVolume";

    private readonly Dictionary<string, AudioClip> bgmClipMap = new(StringComparer.Ordinal);
    private readonly Dictionary<string, AudioClip> seClipMap = new(StringComparer.Ordinal);
    private AudioSource bgmSource;
    private AudioSource seSource;
    private float bgmVolume;
    private float seVolume;

    public float BgmVolume => bgmVolume;
    public float SeVolume => seVolume;
    public bool IsMuted { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildClipMaps();
        CreateAudioSources();
        LoadVolumeSettings();
        if (playDefaultBgmOnAwake)
        {
            PlayBGM(defaultBgm);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void PlayBGM()
    {
        PlayBGM(defaultBgm);
    }

    public void PlayBGM(string key)
    {
        if (!bgmClipMap.TryGetValue(key, out AudioClip clip))
        {
            Debug.LogWarning($"BGM not found: {key}", this);
            return;
        }

        PlayBGM(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Cannot play a null BGM clip.", this);
            return;
        }

        if (bgmSource.clip == clip && bgmSource.isPlaying)
        {
            return;
        }

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    public void PlaySE(string key)
    {
        if (!seClipMap.TryGetValue(key, out AudioClip clip))
        {
            Debug.LogWarning($"SE not found: {key}", this);
            return;
        }

        PlaySE(clip);
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Cannot play a null SE clip.", this);
            return;
        }

        seSource.PlayOneShot(clip);
    }

    public void SetBgmVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
        SaveVolumeSettings();
    }

    public void SetSeVolume(float volume)
    {
        seVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
        SaveVolumeSettings();
    }

    public void SetMuted(bool muted)
    {
        IsMuted = muted;
        ApplyVolumes();
    }

    public void ToggleMute()
    {
        SetMuted(!IsMuted);
    }

    private void BuildClipMaps()
    {
        AddEntriesToMap(bgmClips, bgmClipMap, "BGM");
        AddEntriesToMap(seClips, seClipMap, "SE");
    }

    private void AddEntriesToMap(
        AudioEntry[] entries,
        Dictionary<string, AudioClip> map,
        string category)
    {
        foreach (AudioEntry entry in entries)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.key) || entry.clip == null)
            {
                continue;
            }

            if (!map.TryAdd(entry.key, entry.clip))
            {
                Debug.LogWarning($"Duplicate {category} key: {entry.key}", this);
            }
        }
    }

    private void CreateAudioSources()
    {
        bgmSource = CreateAudioSource("BGM");
        bgmSource.loop = true;
        seSource = CreateAudioSource("SE");
    }

    private AudioSource CreateAudioSource(string sourceName)
    {
        GameObject sourceObject = new(sourceName);
        sourceObject.transform.SetParent(transform);
        AudioSource source = sourceObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        return source;
    }

    private void LoadVolumeSettings()
    {
        bgmVolume = saveVolumeSettings
            ? PlayerPrefs.GetFloat(BgmVolumeKey, initialBgmVolume)
            : initialBgmVolume;
        seVolume = saveVolumeSettings
            ? PlayerPrefs.GetFloat(SeVolumeKey, initialSeVolume)
            : initialSeVolume;
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (bgmSource != null)
        {
            bgmSource.volume = IsMuted ? 0f : bgmVolume;
        }

        if (seSource != null)
        {
            seSource.volume = IsMuted ? 0f : seVolume;
        }
    }

    private void SaveVolumeSettings()
    {
        if (!saveVolumeSettings)
        {
            return;
        }

        PlayerPrefs.SetFloat(BgmVolumeKey, bgmVolume);
        PlayerPrefs.SetFloat(SeVolumeKey, seVolume);
        PlayerPrefs.Save();
    }
}
