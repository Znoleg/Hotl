using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    [SerializeField] private float _timeScale = 1f;
    [SerializeField] private bool _logHealthChange = false;
    [SerializeField] private bool _logAnimationCycleStart = false;
    [SerializeField] private bool _logEnemyStatus = false;
    public static DebugMode Instance { get; private set; }
    public bool IsHealthLogging => _logHealthChange;
    public bool IsAnimationCycleLogging => _logAnimationCycleStart;
    public bool LogEnemyStatus => _logEnemyStatus;

    private void Start()
    {
        Instance = this;
        Time.timeScale = _timeScale;
    }


}
