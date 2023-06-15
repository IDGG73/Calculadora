using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManagerChildSource : MonoBehaviour
{
    [SerializeField] string identifier;
    [Space]
    [SerializeField] bool registerOnStart = true;
    [SerializeField] bool listenToGlobalFactor = true;
    [Space]
    [SerializeField] float defaultVolume = 1f;

    AudioSource source;

    public string Identifier { get { return identifier; } }
    public float DefaultVolume { get { return defaultVolume; } set { defaultVolume = value; } }
    public AudioSource AudioSource { get { return source; } }

    private void Awake() => source = GetComponent<AudioSource>();
    void OnDestroy() => Unregister();

    void Start()
    {
        if (registerOnStart)
            Register();

        ApplyGlobalVolumeFactor();
    }

    public void Register() => AudioManager.RegisterChildSource(this);
    public void Unregister() => AudioManager.UnregisterChildSource(this);

    public void SetVolume(float vol) => source.volume = vol;
    public void SetPitch(float pitch) => source.pitch = pitch;

    public void ApplyGlobalVolumeFactor()
    {
        if (listenToGlobalFactor)
            source.volume = source.volume * AudioManager.globalVolumeFactor;
    }

    public void SetIdentifier(string newIdentifier) => identifier = newIdentifier;
}
