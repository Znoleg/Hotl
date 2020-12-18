using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Damager : MonoBehaviour
{
    public float Damage { get; set; }
    
    private readonly HashSet<int> alreadyHit = new HashSet<int>();
    private bool isEnabled;
    private new Collider2D collider;

    private Transform holderTransform;
    private Character _holderCharacter;
    private const float damageStartDelay = 0.5f;
    private float damagerStart;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        if (!collider.isTrigger)
            throw new UnityException($"Object {gameObject.name} collider must be trigger!");
    }

    public void Begin(Transform holder)
    {
        isEnabled = true;
        damagerStart = Time.fixedTime;
        if (holder) holderTransform = holder;
        if (Damage == 0)
            Debug.LogWarning("Damage is set to 0!");
    }

    public void Begin(Character holder)
    {
        _holderCharacter = holder;
        Begin(holder.transform);
    }

    public void End()
    {
        isEnabled = false;
        alreadyHit.Clear();
        holderTransform = null;
    }


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isEnabled) return;

        var target = collision.gameObject.GetComponent<Target>();
        if (!target || !alreadyHit.Add(target.GetInstanceID())) return;

        if (!DamageHolderCheck(collision)) return;
        //if (!target.Transparent) return;

        if (target is Character character)
            character.SetHiter(_holderCharacter);
        if (target is DestroyableTarget destroyableTarget)
            destroyableTarget.TakeDamage(Damage);
        else target.TakeDamage(Damage);
        
    }

    protected bool DamageHolderCheck(Collider2D collision)
    {
        if (collision.transform == holderTransform && Time.fixedTime - damageStartDelay <= damagerStart)
             return false;
        if (_holderCharacter && (collision.GetComponent<EnemyCharacter>() && _holderCharacter.GetComponent<EnemyCharacter>())) return false;
        return true;
    }
    
}
