using UnityEngine;
using System.Collections;

public class SEProduceCrystals : SpellEffect
{
    public int num;

    public override void Do(Mage caster, Spell origin_spell)
    {
        if (caster.GetCrystalCount() < 6) caster.AddCrystals(6 - caster.GetCrystalCount());
        else caster.AddCrystals(num);
    }
}
