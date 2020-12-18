using UnityEngine.Audio;
using UnityEngine;

public enum SoundType
{
    Music, SFX
}

[System.Serializable]
public class Sound
{
    [SerializeField]
    public string name;

    [SerializeField]
    public AudioClip clip;

    [SerializeField][Range(0f, 1f)]
    public float volume;

    [SerializeField][Range(.1f, 3f)]
    public float pitch;

    [SerializeField]
    public SoundType type;

    [HideInInspector]
    public AudioSource source;
}
