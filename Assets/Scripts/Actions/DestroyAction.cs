using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAction : InteractTarget
{
    [SerializeField] private GameObject toDestroy;
    
    public override void DoAction()
    {
        Destroy(toDestroy);
    }

    private void Start()
    {
        if (!toDestroy) toDestroy = this.gameObject;
    }
}
