using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    [SerializeField] private Animator _playerAnim;
    [SerializeField] private RuntimeAnimatorController _playerContr;
    [SerializeField] private PlayableDirector _director;
    private bool _fix = false;
    
    public static TimelineManager Instance { get; private set; }
    public bool IsPlaying
    {
        get { return _director.state == PlayState.Playing; }
    }

    private void OnEnable()
    {
        _playerContr = _playerAnim.runtimeAnimatorController;
        _playerAnim.runtimeAnimatorController = null;
    }

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        if (_director.state != PlayState.Playing && !_fix)
        {
            _fix = true;
            _playerAnim.runtimeAnimatorController = _playerContr;
        }
    }
}
