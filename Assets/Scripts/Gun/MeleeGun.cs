using Assets.Scripts;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

[Serializable]
[CreateAssetMenu(fileName = "Mellee gun", menuName = "Guns/Mellee")]
[RequireComponent(typeof(Damager))]
public class MeleeGun : Gun
{
    private enum HitType
    {
        Stun, Kill
    }

    [SerializeField] HitType _hitType = HitType.Kill;
    [SerializeField] private float _hitRange = 30f;
    [SerializeField] private float _durability = float.PositiveInfinity;
    private Transform _transform;
    private Damager _damager;
    private CoroutineObject _hitCoroutine;

    //public HitType TypeOfHit => _hitType;
    public float HitRange => _hitRange;

    public static event Action<MeleeGun> OnMeleeGunHit;

    public override bool Shoot()
    {
        if (!base.Shoot()) return false;

        _hitCoroutine.Start();
        
        return true;
    }

    protected override void Start()
    {
        base.Start();
        _transform = transform;
        _damager = GetComponent<Damager>();
        _damager.Damage = Damage;
    }

    private void Awake()
    {
        _hitCoroutine = new CoroutineObject(this, HitCoroutine);
    }

    private void DurabilityDecrease()
    {
        _durability--;
        if (_durability <= 0)
            Destroy(gameObject); // anim
    }

    private IEnumerator DamageOnAttackSprite(DestroyableTarget toDamage)
    {
        yield return new WaitUntil(() => CurrentHolder.CharacterGraphics.GunSpriteHolder.IsCurrentSpriteIsAttack);

        //OnMeleeGunHit?.Invoke(this);
        if (toDamage is Character character)
        {
            character.SetHiter(CurrentHolder);
            if (_hitType == HitType.Stun)
            {
                character.ChangeStatus(NPCStatus.Stunned);
            }
            else character.TakeDamage(Damage);
        }
    }

    private IEnumerator HitCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        
        List<RaycastHit2D> hitObjects = new List<RaycastHit2D>();
        const float hitWidth = 5f;
        Vector2 forwardDir = transform.right;
        Vector2[] directions = { forwardDir, forwardDir.Rotate(hitWidth), forwardDir.Rotate(-hitWidth) };
        foreach (Vector2 direction in directions)
        {
            var hitDirObjects = (Physics2D.RaycastAll(transform.position, direction, _hitRange));
            foreach (var hit in hitDirObjects)
                if (!hitObjects.Find(h => h.transform == hit.transform))
                    hitObjects.Add(hit);
        }

        //var hitObjects = Physics2D.CircleCastAll(transform.position, _thickness, transform.right);

        foreach (var hit in hitObjects)
        {
            #region Hit skip conditions
            if (hit.transform == CurrentHolder.transform) continue;
            if (hit.transform.GetComponent<FriendlyCharacter>() && CurrentHolder.transform.GetComponent<FriendlyCharacter>()
                || hit.transform.GetComponent<EnemyCharacter>() && CurrentHolder.transform.GetComponent<EnemyCharacter>()) continue;
            #endregion
            var body = hit.transform.GetComponent<DestroyableTarget>();
            if (body)
            {
                StartCoroutine(DamageOnAttackSprite(body));
            }
        }
        DurabilityDecrease();

        _hitCoroutine.Stop();
        yield break;
    }
}
