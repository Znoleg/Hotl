using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : GroupedUI
{
    public static DeathScreen Instance { get; private set; }
    private CoroutineObject _resurrectRoutine;

    public override void Activate()
    {
        base.Activate();
        Time.timeScale = 0f;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        Time.timeScale = 1f;
    }

    public void ResurrectPlayerAtCheckpoint()
    {
        _resurrectRoutine.Start();
    }

    private void Awake()
    {
        Instance = this;
        _resurrectRoutine = new CoroutineObject(this, ResurrectPlayer);
    }

    private void Start()
    {
        PlayerCharacter.Instance.OnPlayerDeath += Activate;
    }

    private IEnumerator ResurrectPlayer()
    {
        LevelLoader.Instance.ReloadCurrentScene();

        yield return new WaitUntil(() => LevelLoader.Instance.CurrentScene.isLoaded);
        PlayerCharacter.Instance.transform.position = LevelLoader.CurrentLevel.Checkpoint.position;

        _resurrectRoutine.Stop();
        yield break;
    }
}
