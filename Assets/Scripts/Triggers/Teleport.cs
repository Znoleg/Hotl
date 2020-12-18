using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Trigger : MonoBehaviour
{
    [SerializeField] private List<Collider2D> _triggerColliders = new List<Collider2D>();
    [SerializeField] protected List<Collider2D> TriggerColliders { get { return _triggerColliders; } set { _triggerColliders = value; } }
    
    public abstract void DoAction();

    protected virtual void Start()
    {
        this.CheckColliderTrigger();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_triggerColliders.Find(c => c.transform == collision.transform))
        {
            DoAction();
        }
    }
}

public abstract class PlayerTrigger : Trigger
{
    protected override void Start()
    {
        base.Start();
        TriggerColliders.Add(PlayerCharacter.Instance.GetComponent<Collider2D>());
    }
}

[RequireComponent(typeof(Collider2D))]
public class Teleport : PlayerTrigger
{
    [SerializeField] private bool _active = true;
    [SerializeField] private Transform _destination;
    [SerializeField] private Level toOn, toOff;
    private LevelLoader _levelLoader;

    public bool Active { get { return _active; } set { _active = value; } }

    public override void DoAction()
    {
        if (_levelLoader == null) _levelLoader = LevelLoader.Instance;
        if (_active) StartCoroutine(_levelLoader.Transit(_destination.transform.position, toOn, toOff, this));
    }

    protected override void Start()
    {
        base.Start();
        _levelLoader = LevelLoader.Instance;
        //AstarPath.FindAstarPath();
        AstarPath.active.Scan();
    }
}
