using Assets.Scripts.Systems.Sound;
using EZCameraShake;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowAction : MonoBehaviour, IEventInvoker
{
    [SerializeField] private TextTimer _timerToSubscribeOn;
    [SerializeField] private GameObject _bombToDestroy;
    [SerializeField] private CameraShakerStats _cameraShakerStats;
    [SerializeField] private List<SpriteRendererAnimation> _animations = new List<SpriteRendererAnimation>();
    private CorpseSpawnerBase _objectGraphics;
    private Damager _damager;

    public event Action Event;

    private void Start()
    {
        if (_animations.Count == 0)
        {
            _animations.Add(GetComponent<SpriteRendererAnimation>());
            _animations.Add(GetComponentInChildren<SpriteRendererAnimation>());
        }
        
        _damager = GetComponent<Damager>();
        //_objectGraphics = GetComponent<ObjectGraphics>();

        _damager.Damage = 100f;
        _timerToSubscribeOn.OnTimerStop += Blow;
        //if (!_toSetBlow) _toSetBlow = GetComponentInParent<SpriteRenderer>();
    }

    private void Blow()
    {
        Event?.Invoke();

        var door = GetComponentInParent<Spawner>();
        door.GetComponent<SpriteRenderer>().enabled = false;
        _bombToDestroy = door.InstancedObject;
        Destroy(_bombToDestroy);

        foreach (var animation in _animations)
        {
            animation.AnimationSpriteChanger.Animate();
        }

        #region Finding max animation duration
        float maxDuration = 0f;
        foreach (var animation in _animations)
            if (maxDuration < animation.AnimationSpriteChanger.AnimDuration) maxDuration = animation.AnimationSpriteChanger.AnimDuration;
        #endregion

        StartCoroutine(Utils.WaitCoroutine(() =>
        {
            Destroy(door.gameObject);
            Destroy(gameObject);
        }, maxDuration));

        _damager.Begin(transform);
        StartCoroutine(Utils.WaitCoroutine(_damager.End, 3f));

        CameraShaker.Instance.ShakeOnce(UnityEngine.Random.Range(_cameraShakerStats.forceMin, _cameraShakerStats.forceMax),
            _cameraShakerStats.roughness, _cameraShakerStats.fadeInTime, _cameraShakerStats.fadeOutTime);

        //_objectGraphics.SetVisualDeath();

        //if (_toSetBlow && _blowSprite) _toSetBlow.sprite = _blowSprite;
    }
 
}
