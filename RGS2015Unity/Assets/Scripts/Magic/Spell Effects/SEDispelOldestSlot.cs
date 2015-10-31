using UnityEngine;
using System.Collections;

public class SEDispelOldestSlot : SpellEffect
{
    public override void Do(Mage caster, Spell origin_spell)
    {
        ManaSlot slot = caster.GetOldestFilledManaSlot();
        if (slot.GetProjectile() == null) return;
        slot.Empty(ManaSlotCooldown.Short);
        slot.GetProjectile().Destroy();
        base.Do(caster, origin_spell);
    }
}
