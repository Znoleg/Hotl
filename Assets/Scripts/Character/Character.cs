using Assets.Scripts.Character;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum NPCStatus
{
    Sleep, Walk, Raged, TargetLost, ActiveSearching, Stunned, Died
}

public enum KillType
{
    FromStun, Simple
}

[RequireComponent(typeof(ObjectMovement))]
public abstract class Character : DestroyableTarget, IGunHolder
{
    public enum ColliderChangeMode
    {
        Component, IsTrigger
    }

    [Space]

    [SerializeField] private Gun _gun;
    [SerializeField] private Transform _gunPosition;
    [SerializeField] private bool _dropGunOnDeath = true;
    [SerializeField] private bool _canUseFists;
    [SerializeField] private GameObject _fistsPrefab;

    public NPCStatus CurrentStatus { get; private set; }
    public Transform Transform => transform;
    public NPCGraphics CharacterGraphics { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public Gun CurrentGun => _gun;
    public ObjectMovement BaseMovement { get; private set; }
    public Transform GunPosition => _gunPosition;
    public Transform LastHiter { get; private set; }
    public Vector2 DirectionToFall { get; private set; }
    public bool IsAttacking { get; protected set; }
    public Rigidbody2D Rigitbody { get; private set; }

    public event Action<float> OnDeathAction;
    public event Action<NPCStatus> StatusChanged;
    public event Action OnGunPicked;
    public event Action<Gun> OnCurrentGunShot; // obertka

    public void ChangeStatus(NPCStatus status)
    {
        CurrentStatus = status;
        StatusChanged?.Invoke(status);
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
    }

    protected virtual void PickupGun(Gun newGun)
    {
        //if (_gun)
        //{
        //    DropCurrentGun();
        //}
        if (newGun is FistsGun fistsGun) fistsGun.Pickup(this);
        else newGun.Pickup(this);
        
        SetGun(newGun);
        OnGunPicked?.Invoke();
    }

    protected void DropCurrentGun()
    {
        if (_gun is FistsGun)
        {
            return;
        }
        else _gun.Drop(this);
        
        if (_canUseFists)
        {
            var fists = Instantiate(_fistsPrefab).transform.GetComponent<FistsGun>();
            fists.name = gameObject.name + " " + fists.name;
            PickupGun(fists);
        }
    }

    public void SetHiterDoor()
    {
        DirectionToFall = -transform.right;
    }

    public void SetHiter(Character hiter)
    {
        LastHiter = hiter.transform;
        DirectionToFall = LastHiter.right;
    }

    public void SetHiter(Vector2 dir)
    {
        DirectionToFall = dir;
    }

    public void Push(float force, Vector2 direction, ForceMode2D forceMode = ForceMode2D.Force)
    {
        Rigitbody.AddForce(direction * force * Rigitbody.mass, forceMode);
    }

    protected virtual bool CheckAttack()
    {
        return IsAttacking = BaseMovement.IsTriggered && _gun;
    }
    protected virtual void Attack()
    {
        _gun.Shoot();
    }

    protected virtual void SetPhysicsActive(bool status, ColliderChangeMode mode)
    {
        if (mode == ColliderChangeMode.Component)
        {
            Collider.enabled = status;
        }
        else
        {
            Collider.isTrigger = status;
        }
    }

    protected override void Start()
    {
        base.Start();
        Rigitbody = GetComponent<Rigidbody2D>();
        CharacterGraphics = GetComponent<NPCGraphics>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BaseMovement = GetComponent<ObjectMovement>();
        StartCoroutine(Utils.WaitCoroutine(() =>
        {
            if (_gun)
            {
                _gun.OnShot += InvokeCurrentGunShot;
                _gun.Pickup(this);
                OnGunPicked?.Invoke();
            }
            else if (GetComponentInChildren<Gun>() is Gun gun && gun)
            {
                PickupGun(gun);
            }
        }));

    }

    protected override void Update()
    {
        base.Update();
        if (CheckAttack()) Attack();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (BaseMovement.IsTriggered)
            CheckAttack();
    }

    protected override bool DestroyEvent()
    {
        ChangeStatus(NPCStatus.Died);
        var angle = Vector2.Angle(Vector2.right, DirectionToFall);
        if (DirectionToFall.y < 0) angle = -angle;
        const float corpseRotation = 180f;
        OnDeathAction?.Invoke(angle - corpseRotation);
        if (_dropGunOnDeath)
        {
            if (_gun)
            {
                //var gun = Instantiate(_gun, transform.position, transform.rotation);
                //var sr = gun.GetComponent<SpriteRenderer>();
                //sr.enabled = true;
                //sr.sortingOrder = 4;
                //gun.transform.SetParent(LevelLoader.CurrentLevel.transform, true);
                Vector2 fallDir = transform.right.Rotate(Random.Range(0f, 360f));
                _gun.Drop(this, fallDir, 15f);
            }
        }

        return base.DestroyEvent();
    }
    
    private void InvokeCurrentGunShot()
    {
        OnCurrentGunShot?.Invoke(CurrentGun);
    }

    private void SetGun(Gun gun)
    {
        if (_gun) _gun.OnShot -= InvokeCurrentGunShot;
        _gun = gun;
        _gun.OnShot += InvokeCurrentGunShot;
    }
}
