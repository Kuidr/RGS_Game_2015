using UnityEngine;
using System.Collections;

public class SESpawnProjectile : SpellEffect
{
    public Projectile projectile_prefab;
    public bool use_mana_slot = true;

    public override void Do(Mage caster)
    {
        if (use_mana_slot && !caster.ManaSlotAvailable()) return;

        Projectile p = Instantiate<Projectile>(projectile_prefab);
        p.Initialize(caster, caster.cast_point.position);
        if (use_mana_slot) caster.FillManaSlot(p);
        p.GetRigidbody().AddForce(GeneralHelpers.RandomDirection2D(), ForceMode2D.Impulse);
    }
}
