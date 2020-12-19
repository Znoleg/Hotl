using Assets.Scripts.Character;
using UnityEngine;

public class FistsGun : MeleeGun
{
    public override void Pickup(IGunHolder holder)
    {
        SetRightPickupPosition(holder);
    }

    public override void Drop(IGunHolder holder, Vector2? dropDir = null, float force = 40)
    {
        Destroy(this);
    }
}
