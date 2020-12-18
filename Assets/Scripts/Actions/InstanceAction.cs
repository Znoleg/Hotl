using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Spawner))]
public class InstanceAction : InteractTarget
{
    private Spawner spawner;
   
    public override void DoAction()
    {
        spawner.Spawn(spawner.Prefab, spawner.Position, spawner.Rotation);
    }

    
    private void Start()
    {
        spawner = GetComponent<Spawner>();
    }


}
