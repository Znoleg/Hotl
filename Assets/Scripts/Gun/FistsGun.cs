using Assets.Scripts.Character;

public class FistsGun : MeleeGun
{
    public override void Pickup(IGunHolder holder)
    {
        SetRightPickupPosition(holder);
    }

    public override void Drop(IGunHolder holder)
    {
        Destroy(this);
    }
}
