using Assets.Scripts;
using System;
using UnityEngine;

public class GroupedUI : MonoBehaviour, IGroupedUI
{
    public static event Action<GroupedUI> OnUIOpen;

    public virtual void Activate()
    {
        gameObject.SetActive(true);
        OnUIOpen?.Invoke(this);
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }
}