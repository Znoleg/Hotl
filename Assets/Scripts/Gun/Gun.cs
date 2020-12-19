using Assets.Scripts.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using System;
using System.Linq;
using UnityEngine.Tilemaps;
using DG.Tweening;

[Serializable]
public class CameraShakerStats
{
    public float forceMin = 3f, forceMax = 10f;
    public float roughness;
    public float fadeInTime = .1f;
    public float fadeOutTime = 1f;
}

[RequireComponent(typeof(Collider2D))]
public abstract class Gun : MonoBehaviour, IInteractable
{
    #region fields
    
    [SerializeField] private GunType _gunType;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private float _shootDelay = 1f;
    [SerializeField] private Sound _shootSound;
    [SerializeField] private CameraShakerStats _cameraShakerStats;
    
    private List<Character> _currentCharacters = new List<Character>();
    private float _timeCheck;
    
    private Animator _animator;
    private Collider2D _collider;
    private Rigidbody2D _rigitBody;

    private CoroutineObject makeGunOptainRoutine;

    private Vector2 _flyingDir; 

    #endregion

    #region properites

    public GunType GunType => _gunType;
    public float ShootDelay => _shootDelay;
    public SpriteRenderer SpriteRenderer { get; private set; }
    public CameraShakerStats CameraShakerStats => _cameraShakerStats;
    public bool IsReloaded { get; private set; }
    public Sound ShootSound => _shootSound;
    public Character CurrentHolder { get; private set; }
    public float Damage => _damage;

    #endregion

    public event Action OnPick;
    public event Action OnHit;
    public event Action OnShot;
    public static event Action<Gun> OnGunShot;

    public virtual bool Shoot()
    {
        if (Time.time - _shootDelay <= _timeCheck) return IsReloaded = false;
        _timeCheck = Time.time;

        OnShot?.Invoke();
        OnGunShot?.Invoke(this);

        return IsReloaded = true;
    }

    public bool CanInteract(Character character) => character is PlayerCharacter;

    public void Interact(Character character)
    {
        Pickup(character);
    }


    public virtual void Drop(IGunHolder holder, Vector2? dropDir = null, float force = 40f)
    {
        _collider.enabled = true;
        StartCoroutine(transform.FindMoveDirection(result => _flyingDir = result));
        
        transform.SetParent(holder.Transform.parent, true); // drop // transform.parent = world
        TurnOnGunSpr();
        _rigitBody.bodyType = RigidbodyType2D.Dynamic;
        _collider.isTrigger = false;
        
        Vector2 direction = dropDir.HasValue ? dropDir.Value : (Vector2)transform.right;
        _rigitBody.AddForce(direction * force * _rigitBody.mass, ForceMode2D.Impulse);
        _rigitBody.DORotate(720f, 2f);
        StartCoroutine(MakeGunOptainable());
    }

    public virtual void Pickup(IGunHolder holder)
    {
        _rigitBody.velocity = Vector2.zero;
        _rigitBody.bodyType = RigidbodyType2D.Kinematic;

        TurnOffGunSpr();

        SetRightPickupPosition(holder);

        UpdateIgnoreColliders();

        _collider.enabled = false;

        OnPick?.Invoke();
    }

    protected void SetRightPickupPosition(IGunHolder holder)
    {
        transform.SetParent(holder.Transform, true); // pickup
        transform.localRotation = new Quaternion();

        CurrentHolder = GetComponentInParent<Character>();
        transform.position = CurrentHolder.GunPosition.position;
    }

    protected virtual void Start()
    {
        _collider = GetComponent<Collider2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        _rigitBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        AudioManager.Instance.InitSound(_shootSound);
        _timeCheck = -_shootDelay;
        LevelLoader.OnSubLevelLoad += () => UpdateIgnoreColliders();
    }

    protected virtual void InvokeOnHit()
    {
        OnHit?.Invoke();
    }

    private void Awake()
    {
        makeGunOptainRoutine = new CoroutineObject(this, MakeGunOptainable);
        
    }

    private void TurnOnGunSpr()
    {
        if (SpriteRenderer) SpriteRenderer.enabled = true;
    }
    private void TurnOffGunSpr()
    {
        if (SpriteRenderer) SpriteRenderer.enabled = false;
    }

    private IEnumerator MakeGunOptainable()
    {
        yield return new WaitUntil(() => !_rigitBody.IsFlying());
        _rigitBody.bodyType = RigidbodyType2D.Kinematic;
        _collider.isTrigger = true;
        makeGunOptainRoutine.Stop();
    }

    private void UpdateIgnoreColliders()
    {
        UpdateCharacterList();
        //foreach (Character character in _currentCharacters)
        //    Physics2D.IgnoreCollision(_collider, character.GetComponent<Collider2D>(), true);
        Physics2D.IgnoreCollision(_collider, PlayerCharacter.Instance.GetComponent<Collider2D>(), true);
    }

    private void UpdateCharacterList()
    {
        _currentCharacters = FindObjectsOfType<Character>().OfType<Character>().ToList();
    }

    private void TrySetShineAnim(Collider2D collision)
    {
        if (GetComponentInParent<PlayerCharacter>()) return;
        if (collision.GetComponent<PickupAnimSetter>())
        {
            var raycast = Physics2D.RaycastAll(collision.transform.position, collision.transform.position - transform.position);
            
            foreach (var ray in raycast)
            {
                if (ray.transform == transform) break;
                if (ray.transform.GetComponent<TilemapCollider2D>()) continue;
            }
            _animator.SetBool("playerIsNear", true);
        }
    }

    private void TryOffShineAnim(Collider2D collision)
    {
        if (GetComponentInParent<PlayerCharacter>()) return;
        if (collision.GetComponent<PickupAnimSetter>())
        {
            if (_animator.GetBool("playerIsNear") == true)
                _animator.SetBool("playerIsNear", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    { 
        var enemy = collision.transform.GetComponent<EnemyCharacter>();
        if (enemy && _rigitBody.IsFlying())
        {
            enemy.SetHiter(_flyingDir);
            enemy.ChangeStatus(NPCStatus.Stunned);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TrySetShineAnim(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        TryOffShineAnim(collision);
    }

    
}

