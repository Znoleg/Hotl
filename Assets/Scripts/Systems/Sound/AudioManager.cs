using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] _sounds;

    public static AudioManager Instance { get; private set; }

    public void InitSound(Sound toInit)
    {
        if (toInit == null) return;
        toInit.source = gameObject.AddComponent<AudioSource>();
        toInit.source.clip = toInit.clip;

        toInit.source.volume = toInit.volume;
        toInit.source.pitch = toInit.pitch;
    }

    public void Play(Sound toPlay)
    {
        if (toPlay.source.isPlaying)
        {
            Stop(toPlay);
        }
        
        toPlay.source.Play();
    }

    public void Stop(Sound toStop)
    {
        toStop.source.Stop();
    }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        Gun.OnGunShot += PlayShootSound;
        DontDestroyOnLoad(gameObject);
    }

    private void PlayShootSound(Gun gun)
    {
        Play(gun.ShootSound);
    }

    private void OnDestroy()
    {
        Gun.OnGunShot -= PlayShootSound;
    }
}
