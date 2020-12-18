using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.U2D;

public class DestroyableTarget : Target
{
    protected override bool DestroyEvent()
    {
        if (!base.DestroyEvent()) return false;
        Destroy(gameObject);
        return true;
    }
}

public abstract class Target : PhysicalBody
{
    [SerializeField] protected float m_health;

    public bool HasDied { get; private set; } = false;

    public virtual void TakeDamage(float amount)
    {
        if (HasDied) return;
        m_health -= amount;
        if (DebugMode.Instance.IsHealthLogging) Debug.Log(m_health);
        if (m_health <= 0)
            DestroyEvent();//StartCoroutine(DestroyEvent()); //sobitie
    }

    /// <summary>
    /// Base sets HadDied to true
    /// </summary>
    protected virtual bool DestroyEvent()
    {
        if (HasDied) return false;
        
        return HasDied = true;
        //Destroy(gameObject);
    }

    
}
