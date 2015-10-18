using UnityEngine;
using System.Collections;

public class SESpawnProjectile : SpellEffect
{
    public Projectile projectile_prefab;

    public override void Do(Mage caster)
    {
        if (!caster.ManaSlotAvailable()) return;

        Projectile p = Instantiate<Projectile>(projectile_prefab);
        p.Initialize(caster, caster.cast_point.position);
        caster.FillManaSlot(p);
        p.GetRigidbody().AddForce(GeneralHelpers.RandomDirection2D(), ForceMode2D.Impulse);
    }
}
