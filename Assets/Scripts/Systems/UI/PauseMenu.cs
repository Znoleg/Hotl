using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : GroupedUI
{ 
    public static PauseMenu Instance { get; private set; }
    public static bool IsGamePaused { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public override void Activate()
    {
        base.Activate();
        IsGamePaused = true;
        Time.timeScale = 0f;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        IsGamePaused = false;
        Time.timeScale = 1f;
    }
}
