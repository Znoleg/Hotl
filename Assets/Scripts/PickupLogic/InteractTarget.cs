using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(Collider2D))]
public abstract class InteractTarget : MonoBehaviour
{
    private TimelineManager _timelineManager;

    public abstract void DoAction();

    private void Start()
    {
        StartCoroutine(Utils.WaitCoroutine(() => _timelineManager = TimelineManager.Instance));
    }

    public void Interact()
    {
        if (_timelineManager && _timelineManager.IsPlaying) return;
        DoAction();
    }
}
