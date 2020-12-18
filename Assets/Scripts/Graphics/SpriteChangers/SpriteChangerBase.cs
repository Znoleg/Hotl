using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpriteChangerBase : ISpriteChanger
{
    #region fields

    protected readonly MonoBehaviour _behaviour;

    private Coroutine _animCoroutine;

    #endregion

    public SpriteChangerBase(MonoBehaviour behaviour)
    {
        _behaviour = behaviour;
        SpriteRenderer = _behaviour.GetComponent<SpriteRenderer>();
        if (!SpriteRenderer)
        {
            SpriteRenderer = _behaviour.GetComponentInParent<Character>().GetComponent<SpriteRenderer>();
        }
    }

    #region properties

    public List<Sprite> Sprites { get; 
        set; } = new List<Sprite>();

    public float Delay { get; set; } = 0.09f;

    public bool FinishOnStop { get; set; } = true;

    public int CurrentSpriteIndex { get; protected set; }

    public float AnimDuration => Delay * Sprites.Count;

    public bool IsLaunched => _animCoroutine != null;

    protected SpriteRenderer SpriteRenderer { get; set; }

    #endregion

    #region methods

    public virtual void Update(float deltaTime)
    {

    }

    public virtual void Deinitialize()
    {
        Stop(finish: false);
    }

    public virtual void Launch(
        int? cyclesAmount = null,
        bool setFirstSpriteAfter = false,
        bool continueIfLaunched = false,
        Action onComplete = null)
    {
        if (IsLaunched)
        {
            if (continueIfLaunched)
            {
                return;
            }
            else
            {
                Stop(finish: false);
            }
        }
        _animCoroutine = _behaviour.StartCoroutine(DoAnimCycles(cyclesAmount, setFirstSpriteAfter, onComplete));
    }

    public void Stop(bool? finish = null)
    {
        if (!IsLaunched) return;
        _behaviour.StopCoroutine(_animCoroutine);
        _animCoroutine = null;
        if (finish ?? FinishOnStop)
        {
            while (CurrentSpriteIndex != 0)
            {
                ChangeSprite();
            }
        }
        CurrentSpriteIndex = 0;
    }

    public virtual void SetSprite(Sprite sprite)
    {
        if (IsLaunched)
        {
            Stop(finish: false);
        }
        SpriteRenderer.sprite = sprite;
    }

    protected virtual void ChangeSprite()
    {
        if (Sprites.Count == 0) return;
        CurrentSpriteIndex++;
        if (CurrentSpriteIndex >= Sprites.Count)
            CurrentSpriteIndex = 0;

        if (Sprites.Count == 0) return;
        SpriteRenderer.sprite = Sprites[CurrentSpriteIndex];
    }

    private IEnumerator DoAnimCycles(int? cyclesAmount, bool setFirstSpriteAfter, Action onComplete) // BAD!!!!!!!!!!!!!!!
    {
        if (DebugMode.Instance.IsAnimationCycleLogging) Debug.Log("Animation started on" + _behaviour.gameObject.name);
        var iterations = cyclesAmount ?? float.PositiveInfinity;
        for (int i = 0; i < iterations; i++)
        {

            for (; CurrentSpriteIndex < Sprites.Count; CurrentSpriteIndex++)
            {
                SpriteRenderer.sprite = Sprites[CurrentSpriteIndex];
                yield return new WaitForSeconds(Delay);
            }
            CurrentSpriteIndex = 0;
            if (setFirstSpriteAfter && Sprites.Count > 0)
            {
                SpriteRenderer.sprite = Sprites[CurrentSpriteIndex];
                yield return new WaitForSeconds(Delay);
            }
            yield return null;
        }
        onComplete?.Invoke();
        _animCoroutine = null;

        yield break;
    }

    #endregion
}
