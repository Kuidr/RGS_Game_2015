using UnityEngine;
using System.Collections;

public class SEProduceCrystals : SpellEffect
{
    public int num;

    public override void Do(Mage caster, Spell origin_spell)
    {
        caster.AddCrystals(num);
    }
}
