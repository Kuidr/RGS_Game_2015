using UnityEngine;
using System.Collections;

public class SESpawnProjectile : SpellEffect
{
    public override int FreeSlotsRequired
    {
        get
        {
            return 1;
        }
    }

    public Projectile projectile_prefab;

    public override void Do(Mage caster, Spell origin_spell)
    {
        if (projectile_prefab.GetComponent<ControlledProjectile>() != null
            && !caster.ManaSlotAvailable())
            return;

        Projectile p = Instantiate<Projectile>(projectile_prefab);
        p.Initialize(caster, caster.cast_point.position);

        ControlledProjectile cp = p as ControlledProjectile;
        if (cp != null) caster.FillManaSlot(cp, origin_spell);

        p.GetRigidbody().AddForce(GeneralHelpers.RandomDirection2D(), ForceMode2D.Impulse);
    }
}
