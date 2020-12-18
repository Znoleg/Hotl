using Assets.Scripts.Systems.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHolder : MonoBehaviour
{
    [SerializeField] private Sound _sound;
    [SerializeField] private IEventInvoker _eventInvoker;

    public Sound Sound => _sound;

    private void Start()
    {
        AudioManager.Instance.InitSound(_sound);
        _eventInvoker.Event += PlaySound;
    }

    private void OnDestroy()
    {
        _eventInvoker.Event -= PlaySound;
    }

    private void PlaySound()
    {
        if (AudioManager.Instance) AudioManager.Instance.Play(_sound);
    }

}
