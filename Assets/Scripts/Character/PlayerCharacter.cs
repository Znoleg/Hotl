using EZCameraShake;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerCharacter : Character
{ 
    #region fields
    [SerializeField] private Joystick _shootJoy;
    [SerializeField] private Button _shootButton;
    [SerializeField] private Button _pickupButton;
    [SerializeField] private DeathScreen _screenOnDeath;
    private Gun _pickupGun;
    private const float _pickRadius = 2f;
    private const float _interactRadius = 2f;

    public PlayerGraphics PlayerGraphics { get; private set; }

    #endregion

    public static PlayerCharacter Instance { get; private set; }

    public event Action OnPlayerDeath;

    #region Methods

    #region Public Methods

    public void AttackByButton()
    {
        Attack();
    }

    // called by pickupButton
    public void TryPickupGun()
    {
        if (_pickupGun)
        {
            PickupGun(_pickupGun);
        }
    }

    public void TryDropGun()
    {
        if (CurrentGun)
        {
            DropCurrentGun();
        }
    }

    public void TryInteract()
    {
        if (FindInteract(out InteractTarget[] interacts))
        {
            foreach (var interact in interacts) interact.Interact();
        }
    }

    public void TryKillStunned()
    {
        if (FindStunned(out EnemyCharacter stunned))
        {
            PlayerGraphics.StunKillAnimation.StartKill(stunned.EnemyGraphics);
            Vector2 enemyLoc = stunned.transform.position;
            transform.position = enemyLoc;
            //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, stunned.transform.rotation.z - 180f);
        }
    }

    private bool FindStunned(out EnemyCharacter stunned)
    {
        stunned = null;
        var overlappedColliders = Physics2D.OverlapCircleAll(transform.position, _interactRadius);
        float? minDistance = null;
        foreach (var collider in overlappedColliders)
        {
            EnemyCharacter enemy = collider.GetComponent<EnemyCharacter>();
            if (!enemy || enemy.CurrentStatus != NPCStatus.Stunned || enemy.Rigitbody.IsFlying()) continue;

            var distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (!minDistance.HasValue || distance < minDistance.Value)
            {
                stunned = enemy;
                minDistance = distance;
            }

        }
        return minDistance.HasValue;
    }

    private bool FindInteract(out InteractTarget[] foundInteract)
    {
        foundInteract = null;
        var overlappedColliders = Physics2D.OverlapCircleAll(transform.position, _interactRadius);
        float? minDistance = null;
        foreach (var collider in overlappedColliders)
        {
            var tempInteract = collider.GetComponents<InteractTarget>();

            if (tempInteract.Length > 0)
            {
                var distance = Vector2.Distance(transform.position, tempInteract[0].transform.position);
                if (!minDistance.HasValue || distance < minDistance.Value)
                {
                    foundInteract = tempInteract;
                    minDistance = distance;
                }
            }
        }
        return minDistance.HasValue;
    }

    #endregion

    #region Protected Methods

    protected override void Start()
    {
        base.Start();
        PlayerGraphics = GetComponent<PlayerGraphics>();
    }

    protected override bool DestroyEvent()
    {
        OnPlayerDeath?.Invoke();
        return base.DestroyEvent();
    }

    protected override void Update()
    {
        base.Update();
        RotateByJoy();
        Debug.Log(transform.right);
        var overlappedColliders = Physics2D.OverlapCircleAll(transform.position, _pickRadius);
        if (TryGetClosestGun(overlappedColliders, out var foundGun))
        {
            _pickupGun = foundGun;
        }
        else
        {
            _pickupGun = null;
        }
    }

    protected override void Attack()
    {
        base.Attack();

        if (!CurrentGun.IsReloaded) return;
        if (CurrentGun is ProjectileGun projGun) FireSound.Instance.DoSound(projGun.SoundRange);
        var camShakerStats = CurrentGun.CameraShakerStats;
        CameraShaker.Instance.ShakeOnce(UnityEngine.Random.Range(camShakerStats.forceMin, camShakerStats.forceMax),
            camShakerStats.roughness, camShakerStats.fadeInTime, camShakerStats.fadeOutTime);
    }

    protected override bool CheckAttack()
    {
        return false;
    }

    #endregion

    #region Private Methods

    private bool TryGetClosestGun(IEnumerable<Collider2D> overlappedColliders, out Gun foundGun)
    {
        foundGun = null;
        float? minDistance = null;
        foreach (var collider in overlappedColliders)
        {
            var tempGun = collider.GetComponent<Gun>();

            if (tempGun && tempGun != CurrentGun && !tempGun.GetComponentInParent<Character>())
            {

                Debug.DrawRay(transform.position, tempGun.transform.position - transform.position, Color.red, 1f);
                var raycast = Physics2D.RaycastAll(transform.position, tempGun.transform.position - transform.position);
                foreach (var ray in raycast)
                {
                    if (ray.transform == tempGun.transform) break; 
                    if (ray.transform.GetComponent<TilemapCollider2D>()) continue;
                }

                var distance = Vector2.Distance(transform.position, tempGun.transform.position);
                if (!minDistance.HasValue || distance < minDistance.Value)
                {
                    foundGun = tempGun;
                    minDistance = distance;
                }
                
            }
        }
        return minDistance.HasValue;
    }

    private void RotateByJoy()
    {
        float horiz = _shootJoy.Horizontal;
        float vertical = _shootJoy.Vertical;

        if (horiz != 0 && vertical != 0)
        {
            float heading = Mathf.Atan2(vertical, horiz);
            transform.rotation = Quaternion.Euler(0f, 0f, heading * Mathf.Rad2Deg);
        }
    }

    

    private void Awake()
    {
        Instance = this;
    }



    #endregion

    #endregion Methods

}