using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Sound _chillMusic;
    [SerializeField] private Sound _musicAfterKill;
    private AudioManager _audioManager;
    public static GameManager Instance { get; private set; }
    public static float KillCount { get; private set; }

    public static event Action OnScreenTouch;
    
    public void StopMusic()
    {
        if (_chillMusic != null && _chillMusic.source != null) AudioManager.Instance.Stop(_chillMusic);
        if (_musicAfterKill != null && _musicAfterKill.source != null) AudioManager.Instance.Stop(_musicAfterKill);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AudioManager am = AudioManager.Instance;
        am.InitSound(_chillMusic);
        am.InitSound(_musicAfterKill);

        if (_chillMusic.clip && _musicAfterKill.clip)
        {
            StartCoroutine(Utils.WaitCoroutine(() =>
           {
               _audioManager = AudioManager.Instance;
               if (!_audioManager) return;

               EnemyCharacter.EnemyDied += IncreaseKillCount;
               _audioManager.Play(_chillMusic);
           }));
        }

        LevelLoader.OnMenuMenuLoad += StopMusic;
    }

    private void IncreaseKillCount()
    {
        if (!_audioManager) return;

        if (_chillMusic.clip == _musicAfterKill.clip) return;
        KillCount += 1;
        if (KillCount == 1)
        {
            _audioManager.Stop(_chillMusic);
            _audioManager.Play(_musicAfterKill);
        }
    }

    private void OnDestroy()
    {
        LevelLoader.OnMenuMenuLoad -= StopMusic;
    }
}
