using UnityEngine;
using System.Collections;

public class SESpawnProjectile : SpellEffect
{
    public Projectile projectile_prefab;

    public override void Do(Mage caster)
    {
        if (projectile_prefab.GetComponent<ControlledProjectile>() != null
            && !caster.ManaSlotAvailable())
            return;

        Projectile p = Instantiate<Projectile>(projectile_prefab);
        p.Initialize(caster, caster.cast_point.position);

        ControlledProjectile cp = p as ControlledProjectile;
        if (cp != null) caster.FillManaSlot(cp);

        p.GetRigidbody().AddForce(GeneralHelpers.RandomDirection2D(), ForceMode2D.Impulse);
    }
}
