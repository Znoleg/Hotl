using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class Transition
{
    [SerializeField] private Vector3 _teleportA, _teleportB;
    [SerializeField] private Level _lvlToOff, _lvlToOn;

    public Vector3 TeleportA => _teleportA;
    public Vector3 TeleportB => _teleportB;
    public Level LevelA => _lvlToOff;
    public Level LevelB => _lvlToOn;
}

[Serializable]
public enum SceneIndexes
{
    MainMenu, Level1
}

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GroupedUI _loadingScreen;
    [SerializeField] private Slider _slider;
    [SerializeField] private Animator _transitionAnim;
    [SerializeField] private Level _startingLevel;

    public static LevelLoader Instance { get; private set; }
    public static Level CurrentLevel { get; private set; }

    public static event Action OnSubLevelLoad;
    public static event Action OnMenuMenuLoad;

    public Scene CurrentScene { get; private set; }

    
    public void LoadScene(SceneIndexes index)
    {
        StartCoroutine(LoadAsynchronously((int)index));
    }

    public void LoadScene(int index)
    {
        StartCoroutine(LoadAsynchronously(index));
    }

    public void ReloadCurrentScene()
    {
        CurrentScene = SceneManager.GetActiveScene();
        StartCoroutine(LoadAsynchronously(CurrentScene.buildIndex));
    }    

    private IEnumerator LoadAsynchronously(int sceneIndex)
    {
        _loadingScreen.Activate();

        CurrentScene = SceneManager.GetActiveScene();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            _slider.value = progress;

            yield return null;
        }

        OnMenuMenuLoad?.Invoke();
        SceneManager.UnloadSceneAsync(CurrentScene);

        _loadingScreen.Deactivate();
    }

    public IEnumerator Transit(Vector3 pos, Level toOn, Level toOff, Teleport teleport = null)
    {
        //Time.timeScale = 0f;

        if (teleport) teleport.Active = false;
        _transitionAnim.SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        var player = PlayerCharacter.Instance;
        player.transform.position = pos;
        Camera.main.GetComponent<CameraFollowPlayer>().Refresh();

        toOff.gameObject.SetActive(false);
        toOn.gameObject.SetActive(true);

        CurrentLevel = toOn;

        AstarPath.active.Scan();

        PlayerCharacter.Instance.transform.SetParent(toOn.transform);

        if (teleport) teleport.Active = true;

        //Time.timeScale = 1f;

        OnSubLevelLoad?.Invoke();
    }

    public void ExitAppliction()
    {
        Application.Quit();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CurrentLevel = _startingLevel;
    }
}
